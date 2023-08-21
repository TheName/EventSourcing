using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Aggregates.Builders;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates.Retrievers
{
    internal class EventStreamAggregateRetriever : IEventStreamAggregateRetriever
    {
        private readonly IEventStreamRetriever _eventStreamRetriever;
        private readonly IEventStreamAggregateBuilder _eventStreamAggregateBuilder;

        public EventStreamAggregateRetriever(
            IEventStreamRetriever eventStreamRetriever,
            IEventStreamAggregateBuilder eventStreamAggregateBuilder)
        {
            _eventStreamRetriever = eventStreamRetriever ?? throw new ArgumentNullException(nameof(eventStreamRetriever));
            _eventStreamAggregateBuilder = eventStreamAggregateBuilder ?? throw new ArgumentNullException(nameof(eventStreamAggregateBuilder));
        }

        public async Task<object> RetrieveAsync(Type aggregateType, EventStreamId streamId, CancellationToken cancellationToken)
        {
            var eventStream = await _eventStreamRetriever.RetrieveAsync(streamId, cancellationToken).ConfigureAwait(false);

            if (eventStream.EventsWithMetadata.Count == 0)
            {
                return null;
            }

            return _eventStreamAggregateBuilder.Build(aggregateType, eventStream);
        }
    }
}
