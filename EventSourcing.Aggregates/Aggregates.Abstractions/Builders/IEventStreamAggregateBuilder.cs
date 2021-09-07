using System;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Aggregates.Abstractions.Builders
{
    public interface IEventStreamAggregateBuilder
    {
        object Build(Type aggregateType, EventStream eventStream);
    }
}