using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Transport;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusPublisher : IEventSourcingBusPublisher, IDisposable
    {
        private readonly IRabbitMQProducerFactory _rabbitMQProducerFactory;
        private readonly SemaphoreSlim _producerCreationSemaphore = new SemaphoreSlim(1, 1);
        private IRabbitMQProducer<EventStreamEntry> _producer;
        private bool _isDisposed;

        public RabbitMQEventSourcingBusPublisher(IRabbitMQProducerFactory rabbitMQProducerFactory)
        {
            _rabbitMQProducerFactory = rabbitMQProducerFactory ?? throw new ArgumentNullException(nameof(rabbitMQProducerFactory));
        }

        public async Task PublishAsync(EventStreamEntry eventStreamEntry, CancellationToken cancellationToken)
        {
            var producer = await GetOrCreateProducerIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
            await producer.PublishAsync(eventStreamEntry, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            if (_producer is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _isDisposed = true;
        }

        private async Task<IRabbitMQProducer<EventStreamEntry>> GetOrCreateProducerIfNotExistsAsync(CancellationToken cancellationToken)
        {
            if (_producer != null)
            {
                return _producer;
            }
            
            await _producerCreationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_producer != null)
                {
                    return _producer;
                }

                _producer = await _rabbitMQProducerFactory.CreateAsync<EventStreamEntry>(cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                _producerCreationSemaphore.Release();
            }

            return _producer;
        }
    }
}