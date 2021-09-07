using System;
using EventSourcing.Aggregates.Abstractions.Factories;

namespace EventSourcing.Aggregates.Factories
{
    internal class EventSourcingAggregateFactory : IEventSourcingAggregateFactory
    {
        public object Create(Type aggregateType)
        {
            return Activator.CreateInstance(aggregateType, true);
        }
    }
}