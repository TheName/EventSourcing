using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Persistence
{
    internal class EventStreamStagingReader : IEventStreamStagingReader
    {
        private readonly IEventStreamStagingRepository _repository;

        public EventStreamStagingReader(IEventStreamStagingRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<IReadOnlyCollection<EventStreamStagedEntries>> ReadUnmarkedStagedEntriesAsync(
            CancellationToken cancellationToken)
        {
            return await _repository.SelectAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<EventStreamStagedEntries> ReadUnmarkedStagedEntriesAsync(
            EventStreamStagingId stagingId,
            CancellationToken cancellationToken)
        {
            return await _repository.SelectAsync(stagingId, cancellationToken).ConfigureAwait(false);
        }
    }
}