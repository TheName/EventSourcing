using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Transport;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusConsumer : IEventSourcingBusConsumer
    {
        private readonly IRabbitMQConsumerFactory _consumerFactory;
        private readonly SemaphoreSlim _consumerCreationSemaphore = new SemaphoreSlim(1, 1);
        private IRabbitMQConsumer<EventStreamEntry> _rabbitMQConsumer;

        public RabbitMQEventSourcingBusConsumer(IRabbitMQConsumerFactory consumerFactory)
        {
            _consumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
        }
        
        public async Task StartConsuming(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_rabbitMQConsumer != null)
            {
                throw new InvalidOperationException("Consuming has already been started.");
            }

            await _consumerCreationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_rabbitMQConsumer != null)
                {
                    throw new InvalidOperationException("Consuming has already been started.");
                }
                
                cancellationToken.ThrowIfCancellationRequested();
                _rabbitMQConsumer = await _consumerFactory.CreateAsync(consumingTaskFunc, cancellationToken).ConfigureAwait(false);

            }
            finally
            {
                _consumerCreationSemaphore.Release();
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            await _rabbitMQConsumer.StartConsumingAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopConsuming(CancellationToken cancellationToken)
        {
            if (_rabbitMQConsumer == null)
            {
                return;
            }

            await _rabbitMQConsumer.StopConsumingAsync(cancellationToken).ConfigureAwait(false);
            if (_rabbitMQConsumer is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}