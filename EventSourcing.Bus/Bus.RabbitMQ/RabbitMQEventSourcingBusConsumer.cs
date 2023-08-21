using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Extensions;
using EventSourcing.Bus.RabbitMQ.Transport;
using EventSourcing.ValueObjects;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusConsumer : IEventSourcingBusConsumer, IDisposable
    {
        private readonly IRabbitMQConsumerFactory _consumerFactory;
        private readonly SemaphoreSlim _consumerCreationSemaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _disposingCancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stopConsumingCancellationTokenSource = new CancellationTokenSource();
        private IRabbitMQConsumer<EventStreamEntry> _rabbitMQConsumer;
        private bool _isDisposed;

        public RabbitMQEventSourcingBusConsumer(IRabbitMQConsumerFactory consumerFactory)
        {
            _consumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
        }
        
        public async Task StartConsuming(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            CancellationToken cancellationToken)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQEventSourcingBusConsumer));
            }
            
            var linkedToken = CreateLinkedDisposingAndStoppingToken(cancellationToken);
            linkedToken.ThrowIfCancellationRequested();
            if (_rabbitMQConsumer != null)
            {
                throw new InvalidOperationException("Consuming has already been started.");
            }

            await _consumerCreationSemaphore.WaitAsync(linkedToken).ConfigureAwait(false);
            try
            {
                if (_rabbitMQConsumer != null)
                {
                    throw new InvalidOperationException("Consuming has already been started.");
                }
                
                linkedToken.ThrowIfCancellationRequested();
                _rabbitMQConsumer = await _consumerFactory.CreateAsync(consumingTaskFunc, linkedToken).ConfigureAwait(false);

            }
            finally
            {
                _consumerCreationSemaphore.TryRelease();
            }
            
            linkedToken.ThrowIfCancellationRequested();
            await _rabbitMQConsumer.StartConsumingAsync(linkedToken).ConfigureAwait(false);
        }

        public async Task StopConsuming(CancellationToken cancellationToken)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQEventSourcingBusConsumer));
            }
            
            _stopConsumingCancellationTokenSource.Cancel();
            var linkedToken = CreateLinkedDisposingToken(cancellationToken);
            if (_rabbitMQConsumer == null)
            {
                return;
            }

            await _rabbitMQConsumer.StopConsumingAsync(linkedToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _disposingCancellationTokenSource.Cancel();
            _disposingCancellationTokenSource.Dispose();
            _stopConsumingCancellationTokenSource.Dispose();
            _consumerCreationSemaphore.Dispose();
            if (_rabbitMQConsumer is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _isDisposed = true;
        }

        private CancellationToken CreateLinkedDisposingToken(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposingCancellationTokenSource.Token);

            return linkedTokenSource.Token;
        }

        private CancellationToken CreateLinkedDisposingAndStoppingToken(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposingCancellationTokenSource.Token,
                _stopConsumingCancellationTokenSource.Token);

            return linkedTokenSource.Token;
        }
    }
}
