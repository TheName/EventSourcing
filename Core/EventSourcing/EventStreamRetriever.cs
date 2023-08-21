using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Conversion;
using EventSourcing.Persistence;
using EventSourcing.ValueObjects;

namespace EventSourcing
{
    internal class EventStreamRetriever : IEventStreamRetriever
    {
        private readonly IEventStreamReader _eventStreamReader;
        private readonly IEventStreamEventConverter _eventConverter;

        public EventStreamRetriever(
            IEventStreamReader eventStreamReader,
            IEventStreamEventConverter eventConverter)
        {
            _eventStreamReader = eventStreamReader ?? throw new ArgumentNullException(nameof(eventStreamReader));
            _eventConverter = eventConverter ?? throw new ArgumentNullException(nameof(eventConverter));
        }

        public async Task<EventStream> RetrieveAsync(EventStreamId streamId, CancellationToken cancellationToken)
        {
            var entries = await _eventStreamReader.ReadAsync(streamId, cancellationToken).ConfigureAwait(false);
            return new EventStream(streamId, ConvertToEventsWithMetadata(entries));
        }

        private IEnumerable<EventStreamEventWithMetadata> ConvertToEventsWithMetadata(EventStreamEntries entries)
        {
            return entries
                .Select(eventStreamEntry => new EventStreamEventWithMetadata(
                    _eventConverter.FromEventDescriptor(eventStreamEntry.EventDescriptor),
                    eventStreamEntry.ToEventMetadata()));
        }
    }
}
