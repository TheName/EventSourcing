using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Extensions;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQConsumer<T> : IRabbitMQConsumer<T>, IAsyncBasicConsumer, IBasicConsumer, IDisposable
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly TimeSpan DisposingTimeout = TimeSpan.FromSeconds(10);
        // ReSharper disable once StaticMemberInGenericType
        private static readonly TimeSpan DelayInWaitingForThreadsToFinish = TimeSpan.FromMilliseconds(10);

        private readonly ILogger<RabbitMQConsumer<T>> _logger;
        private readonly IRabbitMQConsumingChannel _consumingChannel;
        private readonly Func<T, CancellationToken, Task> _handler;
        private readonly ISerializer _serializer;
        private readonly object _disposingLock = new object();
        private readonly SemaphoreSlim _startConsumingSemaphore = new SemaphoreSlim(1, 1);
        private readonly Guid _consumerId = Guid.NewGuid();
        private string _consumerTag;
        private bool _hasBeenRegisteredSuccessfully;
        private bool _isCancelled;
        private bool _isShutdown;
        private CancellationTokenSource _cancellationTokenSource;
        private long _runningHandlingTasksCounter = 0;
        private bool _disposing;
        private bool _disposed;
        private bool _triedToStartConsuming;

        private bool IsCancellationRequested => _cancellationTokenSource?.IsCancellationRequested ?? true;

        public RabbitMQConsumer(
            IRabbitMQConsumingChannel consumingChannel,
            Func<T, CancellationToken, Task> handler,
            ISerializer serializer,
            ILogger<RabbitMQConsumer<T>> logger)
        {
            _consumingChannel = consumingChannel ?? throw new ArgumentNullException(nameof(consumingChannel));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            if (_triedToStartConsuming)
            {
                throw new InvalidOperationException(
                    $"Already started consuming; cannot start consuming again. RabbitMQConsumer: {this}");
            }

            await _startConsumingSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_triedToStartConsuming)
                {
                    return;
                }
                
                await _consumingChannel.StartConsumingAsync(basicConsumer: this, cancellationToken).ConfigureAwait(false);
                _triedToStartConsuming = true;
            }
            finally
            {
                _startConsumingSemaphore.Release();
            }
        }

        public async Task StopConsumingAsync(CancellationToken cancellationToken)
        {
            await TryToCancelConsumerOrCancelTokenAndWaitForRunningTasksToFinishAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (_disposingLock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposing = true;
                _logger?.LogDebug("Disposing consumer: {Consumer}, Stack trace: {StackTrace}", this, new StackTrace());
                TryToCancelConsumerOrCancelTokenAndWaitForRunningTasksToFinishAsync(CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();

                if (_consumingChannel is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _cancellationTokenSource?.Dispose();

                _disposed = true;
                _disposing = false;
            }
        }

        public override string ToString() =>
            $"RabbitMQConsumer with id {_consumerId} and consumer tag {(string.IsNullOrEmpty(_consumerTag) ? "<NULL>" : _consumerTag)}. Is cancelled: {_isCancelled}, has been registered successfully: {_hasBeenRegisteredSuccessfully}, is shutdown {_isShutdown}, is cancellation requested: {IsCancellationRequested}, current count of running handling tasks {Interlocked.Read(ref _runningHandlingTasksCounter)}, disposing: {_disposing}, disposed {_disposed}, channel: {_consumingChannel}";

        private async Task HandleIncomingMessageAsync(
            SerializableBasicDeliverEventArgs args,
            ReadOnlyMemory<byte> body)
        {
            if (await RejectMessageIfCancellationRequestedAsync(args).ConfigureAwait(false))
            {
                return;
            }

            var deserializedMessage = Deserialize(body, args);
            if (await RejectMessageIfCancellationRequestedAsync(args).ConfigureAwait(false))
            {
                return;
            }

            _logger.LogDebug(
                "Consumed message. Consumer {Consumer}, Args: {@SerializableBasicDeliverEventArgs}, message: {@DeserializedMessage}",
                this,
                args,
                deserializedMessage);

            await HandleMessage(deserializedMessage, args);
            await AcknowledgeMessageAsync(deserializedMessage, args, _cancellationTokenSource.Token);
        }

        private async Task AcknowledgeMessageAsync(T message, SerializableBasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            try
            {
                await _consumingChannel.AcknowledgeAsync(args.DeliveryTag, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Failed to acknowledge handling message. Consumer {Consumer}, Message: {@Message} Args: {@SerializableBasicDeliverEventArgs}",
                    this,
                    message,
                    args);
                
                throw;
            }
            
            _logger.LogDebug(
                "Successfully acknowledged message. Consumer {Consumer}, Message: {@Message} Args: {@SerializableBasicDeliverEventArgs}",
                this,
                message,
                args);
        }

        private async Task HandleMessage(T message, SerializableBasicDeliverEventArgs args)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _handler(message, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Failed to handle message. Consumer {Consumer}, Message: {@Message} Args: {@SerializableBasicDeliverEventArgs}",
                    this,
                    message,
                    args);
                
                throw;
            }

            _logger.LogDebug(
                "Successfully handled message in {ElapsedTime}. Consumer {Consumer}, Message: {@Message} Args: {@SerializableBasicDeliverEventArgs}",
                stopwatch.Elapsed,
                this,
                message,
                args);
        }

        private async Task<bool> RejectMessageIfCancellationRequestedAsync(SerializableBasicDeliverEventArgs args)
        {
            if (!IsCancellationRequested)
            {
                return false;
            }
            
            _logger.LogDebug(
                "Consumer cancellation has been requested; rejecting the message. Consumer {Consumer}, Args: {@SerializableBasicDeliverEventArgs}",
                this,
                args);

            var rejectionCancellationTokenSource = new CancellationTokenSource(DisposingTimeout);
            await _consumingChannel.RejectAsync(args.DeliveryTag, rejectionCancellationTokenSource.Token).ConfigureAwait(false);

            return true;
        }

        private T Deserialize(ReadOnlyMemory<byte> message, SerializableBasicDeliverEventArgs args)
        {
            var messageArray = message.ToArray();
            try
            {
                return (T)_serializer.DeserializeFromUtf8Bytes(messageArray, typeof(T));
            }
            catch (Exception e)
            {
                var messageString = Encoding.UTF8.GetString(messageArray);

                _logger.LogError(
                    e,
                    "Failed to deserialize message body. Consumer {Consumer}, Args: {@SerializableBasicDeliverEventArgs}, Body: {MessageBody}",
                    this,
                    args,
                    messageString);

                throw new Exception("Failed to deserialize message body.", e);
            }
        }

        private async Task CancelAsync(string consumerTag, CancellationToken cancellationToken)
        {
            if (_isCancelled)
            {
                _logger.LogWarning(
                    "Trying to cancel consumer that is already cancelled; will not proceed. Consumer: {Consumer}, provided consumer tag: {ProvidedConsumerTag}, assigned consumer tag: {AssignedConsumerTag}",
                    this,
                    consumerTag,
                    _consumerTag);
                
                return;
            }
            
            if (string.IsNullOrEmpty(_consumerTag))
            {
                if (!_hasBeenRegisteredSuccessfully)
                {
                    _logger.LogInformation(
                        "Trying to cancel consumer that does not have consumer tag assigned as it was not successfully registered as consumer with the broker yet; will not proceed. Consumer: {Consumer}, provided consumer tag: {ProvidedConsumerTag}, assigned consumer tag: {AssignedConsumerTag}",
                        this,
                        consumerTag,
                        _consumerTag);
                }
                else
                {
                    _logger.LogWarning(
                        "Trying to cancel consumer that does not have consumer tag assigned; will not proceed. Consumer: {Consumer}, provided consumer tag: {ProvidedConsumerTag}, assigned consumer tag: {AssignedConsumerTag}",
                        this,
                        consumerTag,
                        _consumerTag);
                }
                
                return;
            }

            if (!string.Equals(consumerTag, _consumerTag, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning(
                    "Trying to cancel consumer, however provided consumer tag does not match the one assigned to this consumer; will not proceed. Consumer: {Consumer}, provided consumer tag: {ProvidedConsumerTag}, assigned consumer tag: {AssignedConsumerTag}",
                    this,
                    consumerTag,
                    _consumerTag);
                
                return;
            }

            _logger.LogInformation(
                "Cancelling consumer. Consumer: {Consumer}, consumer tag: {ProvidedConsumerTag}",
                this,
                consumerTag);
                
            _isCancelled = true;
            _consumerTag = null;
            _cancellationTokenSource.TryCancel(_logger);
            await WaitForRunningTasksToFinishAsync(cancellationToken).ConfigureAwait(false);
            _cancellationTokenSource?.Dispose();
        }

        private async Task ShutdownAsync(CancellationToken cancellationToken)
        {
            if (_isShutdown)
            {
                _logger.LogWarning(
                    "Trying to shut down consumer that is already shutdown; will not proceed. Consumer: {Consumer}",
                    this);
                
                return;
            }
            
            _cancellationTokenSource.TryCancel(_logger);
            await WaitForRunningTasksToFinishAsync(cancellationToken).ConfigureAwait(false);
            _isShutdown = true;
            _consumerTag = null;
        }

        private Task MarkConsumerRegistrationAsSuccessful(string consumerTag)
        {
            _consumerTag = consumerTag;
            _isCancelled = false;
            _hasBeenRegisteredSuccessfully = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            return Task.CompletedTask;
        }

        private async Task TryToCancelConsumerOrCancelTokenAndWaitForRunningTasksToFinishAsync(CancellationToken cancellationToken)
        {
            if (_consumingChannel != null && !string.IsNullOrEmpty(_consumerTag))
            {
                var cancelSucceeded = await _consumingChannel
                    .TryCancelConsumerAsync(_consumerTag, cancellationToken)
                    .ConfigureAwait(false);
            
                if (!cancelSucceeded)
                {
                    _cancellationTokenSource.TryCancel(_logger);
                }
            }
            else
            {
                _cancellationTokenSource.TryCancel(_logger);
            }

            await WaitForRunningTasksToFinishAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task WaitForRunningTasksToFinishAsync(CancellationToken cancellationToken)
        {
            long currentCount = 0;
            var stopwatch = Stopwatch.StartNew();
            while ((currentCount = Interlocked.Read(ref _runningHandlingTasksCounter)) > 0)
            {
                if (stopwatch.Elapsed > DisposingTimeout)
                {
                    _logger.LogWarning(
                        "Waited already for {DisposingTimeout} in order to finalize the shutdown. Finishing the wait even though the current count is still {CurrentCount}, Consumer: {RabbitMQConsumer}",
                        DisposingTimeout,
                        currentCount,
                        this);
                        
                    break;
                }

                _logger.LogInformation(
                    "Waiting for running handlers to exit in order to finalize the shutdown; delaying task for {DelayInWaitingForThreadsToFinish}. Current count: {CurrentCount}, Consumer: {RabbitMQConsumer}",
                    DelayInWaitingForThreadsToFinish,
                    currentCount,
                    this);

                await Task.Delay(DelayInWaitingForThreadsToFinish, cancellationToken).ConfigureAwait(false);
            }
        }

        #region IAsyncBasicConsumer implementation

        async Task IAsyncBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            _logger.LogDebug(
                "Cancelling consumer {RabbitMQConsumer} due to IAsyncBasicConsumer.HandleBasicCancelOk method invocation with consumer tag {ConsumerTag}",
                this,
                consumerTag);

            var waitingTimeout = new CancellationTokenSource(DisposingTimeout);
            await CancelAsync(consumerTag, waitingTimeout.Token).ConfigureAwait(false);
        }

        async Task IAsyncBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            _logger.LogDebug(
                "Cancelling consumer {RabbitMQConsumer} due to IAsyncBasicConsumer.HandleBasicCancel method invocation with consumer tag {ConsumerTag}",
                this,
                consumerTag);

            var waitingTimeout = new CancellationTokenSource(DisposingTimeout);
            await CancelAsync(consumerTag, waitingTimeout.Token).ConfigureAwait(false);
        }

        async Task IAsyncBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            _logger.LogDebug(
                "Marking consumer {RabbitMQConsumer} as successfully registered with the broker due to IAsyncBasicConsumer.HandleBasicConsumeOk method invocation with consumer tag {ConsumerTag}",
                this,
                consumerTag);
            
            await MarkConsumerRegistrationAsSuccessful(consumerTag).ConfigureAwait(false);
        }

        async Task IAsyncBasicConsumer.HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body)
        {
            var args = SerializableBasicDeliverEventArgs.Create(
                consumerTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                properties);

            Interlocked.Increment(ref _runningHandlingTasksCounter);
            try
            {
                await HandleIncomingMessageAsync(args, body).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Decrement(ref _runningHandlingTasksCounter);
            }
        }

        async Task IAsyncBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            _logger.LogDebug(
                "Shutting down consumer {RabbitMQConsumer} due to IAsyncBasicConsumer.HandleModelShutdown method invocation with sender {ModelShutdownSender} and reason {ShutdownEventArgs}",
                this,
                model,
                reason);
            
            var waitingTimeout = new CancellationTokenSource(DisposingTimeout);
            await ShutdownAsync(waitingTimeout.Token).ConfigureAwait(false);
        }

        IModel IAsyncBasicConsumer.Model 
        {
            get
            {
                _logger.LogError("An unsupported getter IAsyncBasicConsumer.Model called on {RabbitMQConsumer}", this);

                throw new NotSupportedException($"An unsupported getter IAsyncBasicConsumer.Model called on {this}");
            }
        }

        event AsyncEventHandler<ConsumerEventArgs> IAsyncBasicConsumer.ConsumerCancelled
        {
            add
            {
                _logger.LogError(
                    "An unsupported event handler add method IAsyncBasicConsumer.ConsumerCancelled with value {Value} called on {RabbitMQConsumer}",
                    value, 
                    this);

                throw new NotSupportedException($"An unsupported event handler add method IAsyncBasicConsumer.ConsumerCancelled with value {value} called on {this}");
            }
            remove
            {
                _logger.LogError(
                    "An unsupported event handler remove method IAsyncBasicConsumer.ConsumerCancelled with value {Value} called on {RabbitMQConsumer}",
                    value, 
                    this);

                throw new NotSupportedException($"An unsupported event handler remove method IAsyncBasicConsumer.ConsumerCancelled with value {value} called on {this}");
            }
        }

        #endregion

        #region IBasicConsumer implementation

        void IBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            _logger.LogError(
                "An unsupported method IBasicConsumer.HandleBasicCancel with consumer tag {ConsumerTag} called on {RabbitMQConsumer}",
                consumerTag,
                this);

            throw new NotSupportedException(
                $"An unsupported method IBasicConsumer.HandleBasicCancel with consumer tag {consumerTag} called on {this}");
        }

        void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            _logger.LogError(
                "An unsupported method IBasicConsumer.HandleBasicCancelOk with consumer tag {ConsumerTag} called on {RabbitMQConsumer}",
                consumerTag,
                this);

            throw new NotSupportedException(
                $"An unsupported method IBasicConsumer.HandleBasicCancelOk with consumer tag {consumerTag} called on {this}");
        }

        void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            _logger.LogError(
                "An unsupported method IBasicConsumer.HandleBasicConsumeOk with consumer tag {ConsumerTag} called on {RabbitMQConsumer}",
                consumerTag,
                this);

            throw new NotSupportedException(
                $"An unsupported method IBasicConsumer.HandleBasicConsumeOk with consumer tag {consumerTag} called on {this}");
        }

        void IBasicConsumer.HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body)
        {
            var args = SerializableBasicDeliverEventArgs.Create(
                consumerTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                properties);

            _logger.LogError(
                "An unsupported method IBasicConsumer.HandleBasicDeliver with args {@SerializableBasicDeliverEventArgs} called on {RabbitMQConsumer}",
                args,
                this);

            throw new NotSupportedException(
                $"An unsupported method IBasicConsumer.HandleBasicDeliver with args {args} called on {this}");
        }

        void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            _logger.LogError(
                "An unsupported method IBasicConsumer.HandleModelShutdown with sender {ModelShutdownSender} and reason {ShutdownEventArgs} called on {RabbitMQConsumer}",
                model,
                reason,
                this);

            throw new NotSupportedException(
                $"An unsupported method IBasicConsumer.HandleModelShutdown with with sender {model} and reason {reason} called on {this}");
        }

        IModel IBasicConsumer.Model
        {
            get
            {
                _logger.LogError("An unsupported getter IBasicConsumer.Model called on {RabbitMQConsumer}", this);

                throw new NotSupportedException($"An unsupported getter IBasicConsumer.Model called on {this}");
            }
        }

        event EventHandler<ConsumerEventArgs> IBasicConsumer.ConsumerCancelled
        {
            add
            {
                _logger.LogError(
                    "An unsupported event handler add method IBasicConsumer.ConsumerCancelled with value {Value} called on {RabbitMQConsumer}",
                    value, 
                    this);

                throw new NotSupportedException($"An unsupported event handler add method IBasicConsumer.ConsumerCancelled with value {value} called on {this}");
            }
            remove
            {
                _logger.LogError(
                    "An unsupported event handler remove method IBasicConsumer.ConsumerCancelled with value {Value} called on {RabbitMQConsumer}",
                    value, 
                    this);

                throw new NotSupportedException($"An unsupported event handler remove method IBasicConsumer.ConsumerCancelled with value {value} called on {this}");
            }
        }

        #endregion

        private class SerializableBasicDeliverEventArgs
        {
            public static SerializableBasicDeliverEventArgs Create(
                string consumerTag,
                ulong deliveryTag,
                bool redelivered,
                string exchange,
                string routingKey,
                IBasicProperties properties) =>
                new SerializableBasicDeliverEventArgs
                {
                    ConsumerTag = consumerTag,
                    DeliveryTag = deliveryTag,
                    Redelivered = redelivered,
                    Exchange = exchange,
                    RoutingKey = routingKey,
                    BasicProperties = properties
                };

            public IBasicProperties BasicProperties { get; set; }

            public string ConsumerTag { get; set; }

            public ulong DeliveryTag { get; set; }

            public string Exchange { get; set; }

            public bool Redelivered { get; set; }

            public string RoutingKey { get; set; }

            public override string ToString() =>
                $"ConsumerTag: {ConsumerTag}, DeliveryTag: {DeliveryTag}, Exchange: {Exchange}, Redelivered: {Redelivered}, RoutingKey: {RoutingKey}";
        }
    }
}