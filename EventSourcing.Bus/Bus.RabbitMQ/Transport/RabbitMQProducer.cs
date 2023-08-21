using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Extensions;
using EventSourcing.Serialization;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQProducer<T> : IRabbitMQProducer<T>, IDisposable
    {
        private readonly IRabbitMQProducingChannel _producingChannel;
        private readonly ISerializerProvider _serializerProvider;
        private readonly ILogger<RabbitMQProducer<T>> _logger;
        private readonly Guid _producerId = Guid.NewGuid();
        private readonly CancellationTokenSource _disposingTokenSource = new CancellationTokenSource();
        private readonly object _disposingLock = new object();
        private bool _isDisposed;
        private bool _isDisposing;

        private ISerializer Serializer => _serializerProvider.GetBusSerializer();

        public RabbitMQProducer(
            IRabbitMQProducingChannel producingChannel,
            ISerializerProvider serializerProvider,
            ILogger<RabbitMQProducer<T>> logger)
        {
            _producingChannel = producingChannel ?? throw new ArgumentNullException(nameof(producingChannel));
            _serializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task PublishAsync(T message, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrDisposing();
            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposingTokenSource.Token);
            
            linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var serializedMessage = Serialize(message);
            linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();

            await _producingChannel.PublishAsync(serializedMessage, linkedCancellationTokenSource.Token).ConfigureAwait(false);
        }

        private ReadOnlyMemory<byte> Serialize(T message)
        {
            try
            {
                return Serializer.SerializeToUtf8Bytes(message);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Failed to serialize message. Producer {Producer}, Message: {@Message}",
                    this,
                    message);

                throw new Exception("Failed to serialize message.", e);
            }
        }

        private void ThrowIfDisposedOrDisposing()
        {
            if (_isDisposed)
            {
                var additionalMessage = $"Tried to publish a message using disposed producer. Logging failed due to logger instance being null. Producer: {this}";
                if (_logger != null)
                {
                    _logger.LogError("Tried to publish a message using disposed producer. Producer: {RabbitMQProducer}", this);
                    additionalMessage = $"Tried to publish a message using disposed producer. Producer: {this}";
                }
                
                throw new ObjectDisposedException(nameof(RabbitMQProducer<T>), additionalMessage);
            }

            if (_isDisposing)
            {
                var additionalMessage = $"Tried to publish a message using producer that is currently disposing. Logging failed due to logger instance being null. Producer: {this}";
                if (_logger != null)
                {
                    _logger.LogError("Tried to publish a message using producer that is currently disposing. Producer: {RabbitMQProducer}", this);
                    additionalMessage = $"Tried to publish a message using producer that is currently disposing. Producer: {this}";
                }
                
                throw new ObjectDisposedException(nameof(RabbitMQProducer<T>), additionalMessage);
            }
        }

        public override string ToString() => $"RabbitMQProducer with id {_producerId}, is disposed: {_isDisposed}, channel: {_producingChannel}";

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            lock (_disposingLock)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposing = true;
                _logger?.LogDebug("Disposing producer: {RabbitMQProducer}, Stack trace: {StackTrace}", this, new StackTrace());
                _disposingTokenSource.TryCancel(_logger);
                
                if (_producingChannel is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _disposingTokenSource?.Dispose();

                _isDisposed = true;
                _isDisposing = false;
            }
        }
    }
}