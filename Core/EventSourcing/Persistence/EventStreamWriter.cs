using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Enums;
using EventSourcing.ValueObjects;

namespace EventSourcing.Persistence
{
    internal class EventStreamWriter : IEventStreamWriter
    {
        private readonly IEventStreamRepository _repository;

        public EventStreamWriter(IEventStreamRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<EventStreamWriteResult> WriteAsync(EventStreamEntries eventStreamEntries, CancellationToken cancellationToken)
        {
            return await _repository.WriteAsync(eventStreamEntries, cancellationToken).ConfigureAwait(false);
        }
    }
}
