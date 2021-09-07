using System;

namespace EventSourcing.Aggregates.Abstractions.Factories
{
    public interface IEventSourcingAggregateFactory
    {
        object Create(Type aggregateType);
    }
}