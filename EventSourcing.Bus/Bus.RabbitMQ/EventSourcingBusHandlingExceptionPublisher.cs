using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Extensions;
using EventSourcing.Bus.RabbitMQ.Transport;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class EventSourcingBusHandlingExceptionPublisher : IEventSourcingBusHandlingExceptionPublisher, IDisposable
    {
        private readonly IRabbitMQHandlingExceptionProducerFactory _handlingExceptionProducerFactory;
        private readonly SemaphoreSlim _producerCreationSemaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _disposingCancellationTokenSource = new CancellationTokenSource();
        private IRabbitMQProducer<EventStreamEntryHandlingException> _producer;
        private bool _isDisposed;

        public EventSourcingBusHandlingExceptionPublisher(IRabbitMQHandlingExceptionProducerFactory handlingExceptionProducerFactory)
        {
            _handlingExceptionProducerFactory = handlingExceptionProducerFactory ?? throw new ArgumentNullException(nameof(handlingExceptionProducerFactory));
        }
        
        public async Task PublishAsync(
            EventStreamEntryHandlingException entryHandlingException,
            CancellationToken cancellationToken)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQEventSourcingBusPublisher));
            }
            
            var linkedToken = CreateLinkedToken(cancellationToken);
            
            var producer = await GetOrCreateProducerIfNotExistsAsync(linkedToken).ConfigureAwait(false);
            await producer.PublishAsync(entryHandlingException, linkedToken).ConfigureAwait(false);
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

        private async Task<IRabbitMQProducer<EventStreamEntryHandlingException>> GetOrCreateProducerIfNotExistsAsync(CancellationToken cancellationToken)
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

                _producer = await _handlingExceptionProducerFactory.CreateAsync<EventStreamEntryHandlingException>(cancellationToken)
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