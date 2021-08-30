using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.Abstractions;
using Microsoft.Extensions.Hosting;

namespace EventSourcing.Bus
{
    internal class EventSourcingConsumerHostedService : IHostedService
    {
        private readonly IEventSourcingBusConsumer _consumer;

        public EventSourcingConsumerHostedService(IEventSourcingBusConsumer consumer)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumer.StartConsuming(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consumer.StopConsuming(cancellationToken).ConfigureAwait(false);
        }
    }
}