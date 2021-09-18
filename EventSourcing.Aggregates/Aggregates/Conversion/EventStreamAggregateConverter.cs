using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Abstractions.Conversion;

namespace EventSourcing.Aggregates.Conversion
{
    internal class EventStreamAggregateConverter : IEventStreamAggregateConverter
    {
        public PublishableEventStream ToPublishableEventStream(object aggregate)
        {
            if (aggregate is IEventStreamAggregate eventStreamAggregate)
            {
                return eventStreamAggregate.PublishableEventStream;
            }

            throw new NotSupportedException(
                $"This implementation does not support converting type {aggregate.GetType()} to {typeof(PublishableEventStream)}. Please implement your own {typeof(IEventStreamAggregateConverter)} or make {aggregate.GetType()} implement {typeof(IEventStreamAggregate)}.");
        }
    }
}