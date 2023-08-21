using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Aggregates.Publishers;
using EventSourcing.Aggregates.Retrievers;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates
{
    internal class AggregateEventStore : IAggregateEventStore
    {
        private readonly IEventStreamAggregatePublisher _eventStreamAggregatePublisher;
        private readonly IEventStreamAggregateRetriever _eventStreamAggregateRetriever;

        public AggregateEventStore(
            IEventStreamAggregatePublisher eventStreamAggregatePublisher,
            IEventStreamAggregateRetriever eventStreamAggregateRetriever)
        {
            _eventStreamAggregatePublisher = eventStreamAggregatePublisher ?? throw new ArgumentNullException(nameof(eventStreamAggregatePublisher));
            _eventStreamAggregateRetriever = eventStreamAggregateRetriever ?? throw new ArgumentNullException(nameof(eventStreamAggregateRetriever));
        }
        
        public async Task PublishAsync(object aggregate, CancellationToken cancellationToken)
        {
            await _eventStreamAggregatePublisher.PublishAsync(aggregate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<object> RetrieveAsync(Type aggregateType, EventStreamId streamId, CancellationToken cancellationToken)
        {
            return await _eventStreamAggregateRetriever.RetrieveAsync(aggregateType, streamId, cancellationToken).ConfigureAwait(false);
        }
    }
}
