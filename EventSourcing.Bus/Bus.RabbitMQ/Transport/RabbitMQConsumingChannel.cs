using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Configurations;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQConsumingChannel : IRabbitMQConsumingChannel, IDisposable
    {
        private readonly IRabbitMQChannel _channel;
        private readonly Guid _consumingChannelId = Guid.NewGuid();
        private readonly ILogger<RabbitMQConsumingChannel> _logger;
        private readonly SemaphoreSlim _bindingSemaphore = new SemaphoreSlim(1, 1);
        private string _boundQueueName;

        public RabbitMQConsumingChannel(
            IRabbitMQChannel channel,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task BindQueueAsync(IRabbitMQConsumingQueueBindingConfiguration bindingConfiguration, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_boundQueueName))
            {
                throw new InvalidOperationException(
                    $"This consuming channel is already bound to queue name {_boundQueueName}. Cannot bind another queue to the same consuming channel {this}");
            }

            await _bindingSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (!string.IsNullOrEmpty(_boundQueueName))
                {
                    throw new InvalidOperationException(
                        $"This consuming channel is already bound to queue name {_boundQueueName}. Cannot bind another queue to the same consuming channel {this}");
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


                            model.BasicQos(
                                prefetchSize: 0,
                                bindingConfiguration.PrefetchCount,
                                global: false);
                        },
                        cancellationToken)
                    .ConfigureAwait(false);

                _boundQueueName = bindingConfiguration.QueueName;
            }
            finally
            {
                _bindingSemaphore.Release();
            }

            _logger.LogInformation(
                "Successfully bound queue {QueueName} to exchange {ExchangeName} of type {ExchangeType} using routing {RoutingKey} and prefetch count {PrefetchCount}. Consuming channel: {RabbitMQConsumingChannel}",
                bindingConfiguration.QueueName,
                bindingConfiguration.ExchangeName,
                bindingConfiguration.ExchangeType,
                bindingConfiguration.RoutingKey,
                bindingConfiguration.PrefetchCount,
                this);
        }

        public async Task AcknowledgeAsync(ulong deliveryTag, CancellationToken cancellationToken)
        {
            await _channel.ExecuteActionInThreadSafeMannerAsync(
                    model => model.BasicAck(deliveryTag, multiple: false),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RejectAsync(ulong deliveryTag, CancellationToken cancellationToken)
        {
            await _channel.ExecuteActionInThreadSafeMannerAsync(
                    model => model.BasicReject(deliveryTag, requeue: true),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> TryCancelConsumerAsync(string consumerTag, CancellationToken cancellationToken)
        {
            try
            {
                await _channel.ExecuteActionInThreadSafeMannerAsync(
                        model => model.BasicCancel(consumerTag),
                        cancellationToken)
                    .ConfigureAwait(false);
                
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e,
                    "Failed to cancel consumer with consumer tag: {ConsumerTag}, Consuming channel: {RabbitMQConsumingChannel}",
                    consumerTag,
                    this);
                
                return false;
            }
        }

        public async Task StartConsumingAsync(IBasicConsumer basicConsumer, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_boundQueueName))
            {
                throw new InvalidOperationException(
                    $"Cannot start consuming without the binding the queue. Please bind the queue before you start consuming channel {this}");
            }

            await _channel.ExecuteActionInThreadSafeMannerAsync(
                    model =>
                        model.BasicConsume(
                            _boundQueueName,
                            autoAck: false,
                            consumerTag: string.Empty,
                            noLocal: false,
                            exclusive: false,
                            arguments: null,
                            basicConsumer),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_channel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public override string ToString() =>
            $"RabbitMQConsumingChannel with id {_consumingChannelId} and channel: {_channel}";
    }
}