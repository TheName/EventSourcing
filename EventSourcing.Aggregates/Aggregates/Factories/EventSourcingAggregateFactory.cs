using System;
using System.Linq;
using System.Reflection;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions.Factories;

namespace EventSourcing.Aggregates.Factories
{
    internal class EventSourcingAggregateFactory : IEventSourcingAggregateFactory
    {
        public object Create(Type aggregateType)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException(nameof(aggregateType));
            }
            
            if (aggregateType.IsAbstract)
            {
                throw new ArgumentException($"Provided type ({aggregateType}) is abstract.");
            }

            if (aggregateType.IsInterface)
            {
                throw new ArgumentException($"Provided type ({aggregateType}) is an interface.");
            }
            
            var constructors = aggregateType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (constructors.Length == 0)
            {
                throw new MissingMethodException($"Provided type ({aggregateType}) does not have any constructors");
            }

            var parameterlessConstructor = constructors.SingleOrDefault(info => info.GetParameters().Length == 0);
            if (parameterlessConstructor != null)
            {
                return parameterlessConstructor.Invoke(new object[0]);
            }

            var constructorWithStreamIdOnly = constructors
                .SingleOrDefault(info => info.GetParameters().Length == 1 && info.GetParameters()[0].ParameterType == typeof(EventStreamId));

            if (constructorWithStreamIdOnly != null)
            {
                return constructorWithStreamIdOnly.Invoke(new object[] {EventStreamId.NewEventStreamId()});
            }

            throw new MissingMethodException($"Could not find a valid constructor on type {aggregateType}");
        }
    }
}