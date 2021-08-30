using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Persistence
{
    internal class EventStreamStagingWriter : IEventStreamStagingWriter
    {
        private readonly IEventStreamStagingWriteRepository _writeRepository;

        public EventStreamStagingWriter(IEventStreamStagingWriteRepository writeRepository)
        {
            _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        }

        public async Task<EventStreamStagingId> WriteAsync(EventStreamEntries eventStreamEntries, CancellationToken cancellationToken)
        {
            var stagingId = EventStreamStagingId.NewEventStreamStagingId();
            
            await _writeRepository.InsertAsync(stagingId, eventStreamEntries, cancellationToken).ConfigureAwait(false);

            return stagingId;
        }

        public async Task MarkAsPublishedAsync(EventStreamStagingId streamStagingId, CancellationToken cancellationToken)
        {
            await _writeRepository.DeleteAsync(streamStagingId, cancellationToken).ConfigureAwait(false);
        }

        public async Task MarkAsFailedToStoreAsync(EventStreamStagingId streamStagingId, CancellationToken cancellationToken)
        {
            await _writeRepository.DeleteAsync(streamStagingId, cancellationToken).ConfigureAwait(false);
        }
    }
}