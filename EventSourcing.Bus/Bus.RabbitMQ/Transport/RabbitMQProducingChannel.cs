using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Configurations;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQProducingChannel : IRabbitMQProducingChannel, IDisposable
    {
        private readonly IRabbitMQChannel _channel;
        private readonly ILogger<RabbitMQProducingChannel> _logger;
        private readonly Guid _producingChannelId = Guid.NewGuid();
        private readonly SemaphoreSlim _bindingSemaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _disposingCancellationTokenSource = new CancellationTokenSource();

        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<PublishingResult>> _deliveryTagsWithPublishingResults =
                new ConcurrentDictionary<ulong, TaskCompletionSource<PublishingResult>>();
        
        private IRabbitMQProducingQueueBindingConfiguration _queueBindingConfiguration;

        public RabbitMQProducingChannel(
            IRabbitMQChannel channel,
            ILogger<RabbitMQProducingChannel> logger)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task BindQueueAsync(IRabbitMQProducingQueueBindingConfiguration bindingConfiguration,
            CancellationToken cancellationToken)
        {
            if (_queueBindingConfiguration != null)
            {
                throw new InvalidOperationException(
                    $"This producing channel is already bound to queue {_queueBindingConfiguration}. Cannot bind another queue to the same producing channel {this}");
            }

            await _bindingSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (_queueBindingConfiguration != null)
                {
                    throw new InvalidOperationException(
                        $"This producing channel is already bound to queue {_queueBindingConfiguration}. Cannot bind another queue to the same producing channel {this}");
                }

                await _channel.ExecuteActionInThreadSafeMannerAsync(
                        model =>
                        {
                            model.ExchangeDeclare(
                                bindingConfiguration.ExchangeName,
                                bindingConfiguration.ExchangeType,
                                durable: true,
                                autoDelete: false,
                                arguments: null);

                            model.QueueDeclare(
                                bindingConfiguration.QueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            model.QueueBind(
                                bindingConfiguration.QueueName,
                                bindingConfiguration.ExchangeName,
                                bindingConfiguration.RoutingKey,
                                arguments: null);


                            model.ConfirmSelect();
                            
                            model.BasicAcks += ModelOnBasicAcks;
                            model.BasicNacks += ModelOnBasicNacks;
                        },
                        cancellationToken)
                    .ConfigureAwait(false);

                _queueBindingConfiguration = bindingConfiguration;
            }
            finally
            {
                _bindingSemaphore.Release();
            }

            _logger.LogInformation(
                "Successfully bound queue {QueueName} to exchange {ExchangeName} of type {ExchangeType} using routing {RoutingKey}. ProducingChannel: {RabbitMQProducingChannel}",
                bindingConfiguration.QueueName,
                bindingConfiguration.ExchangeName,
                bindingConfiguration.ExchangeType,
                bindingConfiguration.RoutingKey,
                this);
        }

        public async Task PublishAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<PublishingResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var timeoutTokenSource = new CancellationTokenSource();
            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutTokenSource.Token,
                _disposingCancellationTokenSource.Token);
            
            var linkedToken = linkedCancellationTokenSource.Token;
            ulong? deliveryTag = null;
            
            await _channel.ExecuteActionInThreadSafeMannerAsync(model =>
                    {
                        deliveryTag = model.NextPublishSeqNo;

                        linkedToken.ThrowIfCancellationRequested();
                        var properties = model.CreateBasicProperties();
                        properties.Persistent = true;

                        linkedToken.ThrowIfCancellationRequested();
                        if (!_deliveryTagsWithPublishingResults.TryAdd(deliveryTag.Value, taskCompletionSource))
                        {
                            _logger.LogError(
                                "Registering task completion source for delivery tag {DeliveryTag} failed. Producing Channel: {RabbitMQProducingChannel}",
                                deliveryTag,
                                this);

                            throw new InvalidOperationException(
                                $"Registering task completion source for delivery tag {deliveryTag} failed. Producing Channel: {this}");
                        }

                        linkedCancellationTokenSource.Token.Register(
                            _ => Callback(taskCompletionSource, timeoutTokenSource.Token, deliveryTag.Value),
                            state: null,
                            useSynchronizationContext: false);
                        
                        linkedToken.ThrowIfCancellationRequested();
                        model.BasicPublish(
                            _queueBindingConfiguration.ExchangeName,
                            _queueBindingConfiguration.RoutingKey,
                            false,
                            properties,
                            body);
                    },
                    linkedToken)
                .ConfigureAwait(false);

            timeoutTokenSource.CancelAfter(_queueBindingConfiguration.PublishingTimeout);
            try
            {
                var publishingResult = await taskCompletionSource.Task.ConfigureAwait(false);
                switch (publishingResult)
                {
                    case PublishingResult.Acknowledged:
                        return;
                    
                    case PublishingResult.NotAcknowledged:
                        _logger.LogError(
                            "Publishing message with delivery tag {DeliveryTag} failed with result {PublishingResult}. Producing channel: {RabbitMQProducingChannel}",
                            deliveryTag,
                            publishingResult,
                            this);
                        
                        throw new Exception(
                            $"Publishing message with delivery tag {deliveryTag} failed with result {publishingResult}. Producing channel: {this}");
                    
                    default:
                        throw new NotSupportedException($"Publishing result {publishingResult} is not supported");
                }
            }
            finally
            {
                linkedCancellationTokenSource.Dispose();
                if (deliveryTag != null)
                {
                    _deliveryTagsWithPublishingResults?.TryRemove(deliveryTag.Value, out _);
                }
            }
        }

        private void Callback(
            TaskCompletionSource<PublishingResult> taskCompletionSource,
            CancellationToken timeoutToken,
            ulong deliveryTag)
        {
            _logger.LogDebug(
                "Cancellation has been requested for publishing acknowledgment waiting task (delivery tag: {DeliveryTag}). Producing channel: {RabbitMQProducingChannel}",
                deliveryTag,
                this);

            if (_disposingCancellationTokenSource.IsCancellationRequested)
            {
                if (taskCompletionSource.TrySetException(new TaskCanceledException(
                        $"Cancelling publishing acknowledgment waiting task (delivery tag: {deliveryTag}) due to disposing of Producing channel {this}")))
                {
                    _logger.LogDebug(
                        "Cancelling publishing acknowledgment waiting task (delivery tag: {DeliveryTag}) due to disposing of Producing channel {RabbitMQProducingChannel}",
                        deliveryTag,
                        this);
                }
            }
            else if (timeoutToken.IsCancellationRequested)
            {
                if (taskCompletionSource.TrySetException(
                        new TimeoutException(
                            $"Publishing message failed; acknowledgment waiting task (delivery tag: {deliveryTag}) has reached timeout in {_queueBindingConfiguration.PublishingTimeout} in producing channel {this}")))
                {
                    _logger.LogError(
                        "Publishing message failed; acknowledgment waiting task (delivery tag: {DeliveryTag}) has reached timeout in {PublishingTimeout} in producing channel {RabbitMQProducingChannel}",
                        deliveryTag,
                        _queueBindingConfiguration.PublishingTimeout,
                        this);
                }
            }
            
            if (taskCompletionSource.TrySetException(new TaskCanceledException(
                    $"Cancelling publishing acknowledgment waiting task (delivery tag: {deliveryTag}). Producing channel {this}")))
            {
                _logger.LogDebug(
                    "Cancelling publishing acknowledgment waiting task (delivery tag: {DeliveryTag}). Producing channel {RabbitMQProducingChannel}",
                    deliveryTag,
                    this);
            }
        }

        public void Dispose()
        {
            if (_channel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public override string ToString() =>
            $"RabbitMQProducingChannel with id {_producingChannelId} and channel: {_channel}";

        private void ModelOnBasicAcks(object sender, BasicAckEventArgs e)
        {
            SetDeliveryTagResult(e.DeliveryTag, e.Multiple, PublishingResult.Acknowledged);
        }

        private void ModelOnBasicNacks(object sender, BasicNackEventArgs e)
        {
            SetDeliveryTagResult(e.DeliveryTag, e.Multiple, PublishingResult.NotAcknowledged);
        }

        private void SetDeliveryTagResult(ulong deliveryTag, bool multiple, PublishingResult publishingResult)
        {
            var deliveryTags = new List<ulong> { deliveryTag };
            if (multiple)
            {
                deliveryTags.AddRange(_deliveryTagsWithPublishingResults.Keys.Where(arg => arg <= deliveryTag));
                deliveryTags = deliveryTags
                    .Distinct()
                    .ToList();
            }
            
            deliveryTags.ForEach(tag =>
            {
                if (_deliveryTagsWithPublishingResults.TryRemove(tag, out var taskCompletionSource))
                {
                    if (!taskCompletionSource.TrySetResult(publishingResult))
                    {
                        _logger.LogDebug(
                            "Failed to set publishing result {PublishingResult} for delivery tag {DeliveryTag} (multiple: {Multiple}). Producing channel {RabbitMQProducingChannel}",
                            publishingResult,
                            tag,
                            multiple,
                            this);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "Successfully set publishing result {PublishingResult} for delivery tag {DeliveryTag} (multiple: {Multiple}). Producing channel {RabbitMQProducingChannel}",
                            publishingResult,
                            tag,
                            multiple,
                            this);
                    }
                }
                else
                {
                    _logger.LogDebug(
                        "Could not find task completion source; tried to set publishing result {PublishingResult} for delivery tag {DeliveryTag} (multiple: {Multiple}). Producing channel {RabbitMQProducingChannel}",
                        publishingResult,
                        tag,
                        multiple,
                        this);
                }
            });
        }
        
        private enum PublishingResult
        {
            Undefined,
            Acknowledged,
            NotAcknowledged
        }
    }
}