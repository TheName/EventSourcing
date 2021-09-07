using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions;

namespace EventSourcing.Persistence
{
    internal class EventStreamReader : IEventStreamReader
    {
        private readonly IEventStreamRepository _eventStreamRepository;

        public EventStreamReader(IEventStreamRepository eventStreamRepository)
        {
            _eventStreamRepository = eventStreamRepository ?? throw new ArgumentNullException(nameof(eventStreamRepository));
        }
        
        public async Task<EventStreamEntries> ReadAsync(EventStreamId streamId, CancellationToken cancellationToken)
        {
            return await _eventStreamRepository.ReadAsync(streamId, cancellationToken).ConfigureAwait(false);
        }
    }
}