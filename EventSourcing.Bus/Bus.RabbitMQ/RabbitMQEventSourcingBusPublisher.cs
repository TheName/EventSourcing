using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Extensions;
using EventSourcing.Bus.RabbitMQ.Transport;
using EventSourcing.ValueObjects;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusPublisher : IEventSourcingBusPublisher, IDisposable
    {
        private readonly IRabbitMQProducerFactory _rabbitMQProducerFactory;
        private readonly SemaphoreSlim _producerCreationSemaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _disposingCancellationTokenSource = new CancellationTokenSource();
        private IRabbitMQProducer<EventStreamEntry> _producer;
        private bool _isDisposed;

        public RabbitMQEventSourcingBusPublisher(IRabbitMQProducerFactory rabbitMQProducerFactory)
        {
            _rabbitMQProducerFactory = rabbitMQProducerFactory ?? throw new ArgumentNullException(nameof(rabbitMQProducerFactory));
        }

        public async Task PublishAsync(EventStreamEntry eventStreamEntry, CancellationToken cancellationToken)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQEventSourcingBusPublisher));
            }
            
            var linkedToken = CreateLinkedToken(cancellationToken);
            
            var producer = await GetOrCreateProducerIfNotExistsAsync(linkedToken).ConfigureAwait(false);
            await producer.PublishAsync(eventStreamEntry, linkedToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _disposingCancellationTokenSource.Cancel();
            _disposingCancellationTokenSource.Dispose();
            _producerCreationSemaphore.Dispose();
            if (_producer is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _isDisposed = true;
        }

        private CancellationToken CreateLinkedToken(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposingCancellationTokenSource.Token);

            return linkedTokenSource.Token;
        }

        private async Task<IRabbitMQProducer<EventStreamEntry>> GetOrCreateProducerIfNotExistsAsync(CancellationToken cancellationToken)
        {
            if (_producer != null)
            {
                return _producer;
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            await _producerCreationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_producer != null)
                {
                    return _producer;
                }

                _producer = await _rabbitMQProducerFactory.CreateAsync<EventStreamEntry>(cancellationToken)
                    .ConfigureAwait(false);
                
                cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                _producerCreationSemaphore?.TryRelease();
            }

            return _producer;
        }
    }
}
