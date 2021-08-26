using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Persistence.Abstractions;

namespace EventSourcing.Persistence
{
    internal class EventStreamWriter : IEventStreamWriter
    {
        private readonly IEventStreamWriteRepository _writeRepository;

        public EventStreamWriter(IEventStreamWriteRepository writeRepository)
        {
            _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        }
        
        public async Task<EventStreamWriteResult> WriteAsync(EventStreamEntries eventStreamEntries, CancellationToken cancellationToken)
        {
            return await _writeRepository.WriteAsync(eventStreamEntries, cancellationToken).ConfigureAwait(false);
        }
    }
}