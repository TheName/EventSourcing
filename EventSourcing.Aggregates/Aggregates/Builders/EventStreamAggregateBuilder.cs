using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Abstractions.Builders;
using EventSourcing.Aggregates.Abstractions.Factories;

namespace EventSourcing.Aggregates.Builders
{
    internal class EventStreamAggregateBuilder : IEventStreamAggregateBuilder
    {
        private readonly IEventSourcingAggregateFactory _aggregateFactory;

        public EventStreamAggregateBuilder(IEventSourcingAggregateFactory aggregateFactory)
        {
            _aggregateFactory = aggregateFactory ?? throw new ArgumentNullException(nameof(aggregateFactory));
        }
        
        public object Build(Type aggregateType, EventStream eventStream)
        {
            var aggregate = _aggregateFactory.Create(aggregateType);
            if (aggregate is null)
            {
                throw new NotSupportedException($"Aggregate factory ({_aggregateFactory.GetType()}) returned NULL for aggregate type of {aggregateType}. Cannot build an aggregate out of NULL.");
            }
            
            if (!(aggregate is IEventStreamAggregate eventStreamAggregate))
            {
                throw new NotSupportedException($"This implementation does not support building aggregates of type {aggregateType}. Please implement your own {typeof(IEventStreamAggregateBuilder)} or make {aggregate.GetType()} implement {typeof(IEventStreamAggregate)}.");
            }
            
            eventStreamAggregate.ReplayEventStream(eventStream);
            return eventStreamAggregate;
        }
    }
}