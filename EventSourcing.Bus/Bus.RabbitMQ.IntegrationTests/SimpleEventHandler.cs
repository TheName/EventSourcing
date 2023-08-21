using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Handling;
using EventSourcing.ValueObjects;

namespace Bus.RabbitMQ.IntegrationTests
{
    public class SimpleEventHandler : IEventHandler<SimpleEvent>
    {
        private readonly ConcurrentDictionary<Guid, int> _handledEvents = new();

        public int GetNumberOfHandledEvents(Guid eventId)
        {
            return _handledEvents[eventId];
        }

        public Task HandleAsync(SimpleEvent @event, EventStreamEventMetadata eventMetadata, CancellationToken cancellationToken)
        {
            _handledEvents.AddOrUpdate(@event.EventId, 1, (_, i) => i + 1);
            return Task.CompletedTask;
        }
    }
}
