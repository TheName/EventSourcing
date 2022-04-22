using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Persistence
{
    internal class EventStreamStagingWriter : IEventStreamStagingWriter
    {
        private readonly IEventStreamStagingRepository _repository;

        public EventStreamStagingWriter(IEventStreamStagingRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<EventStreamStagingId> WriteAsync(EventStreamEntries eventStreamEntries, CancellationToken cancellationToken)
        {
            var stagedEntries = new EventStreamStagedEntries(
                EventStreamStagingId.NewEventStreamStagingId(),
                EventStreamStagedEntriesStagingTime.Now(),
                eventStreamEntries);
            
            await _repository.InsertAsync(stagedEntries, cancellationToken).ConfigureAwait(false);

            return stagedEntries.StagingId;
        }

        public async Task MarkAsPublishedAsync(EventStreamStagingId streamStagingId, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(streamStagingId, cancellationToken).ConfigureAwait(false);
        }

        public async Task MarkAsFailedToStoreAsync(EventStreamStagingId streamStagingId, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(streamStagingId, cancellationToken).ConfigureAwait(false);
        }
    }
}