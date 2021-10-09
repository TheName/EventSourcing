using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Bus.Abstractions;
using Microsoft.Extensions.Hosting;

namespace EventSourcing.Bus
{
    internal class EventSourcingConsumerHostedService : IHostedService
    {
        private readonly IEventSourcingBusConsumer _consumer;
        private readonly IEventStreamEntryDispatcher _entryDispatcher;

        public EventSourcingConsumerHostedService(
            IEventSourcingBusConsumer consumer,
            IEventStreamEntryDispatcher entryDispatcher)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _entryDispatcher = entryDispatcher ?? throw new ArgumentNullException(nameof(entryDispatcher));
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumer.StartConsuming(_entryDispatcher.DispatchAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consumer.StopConsuming(cancellationToken).ConfigureAwait(false);
        }
    }
}