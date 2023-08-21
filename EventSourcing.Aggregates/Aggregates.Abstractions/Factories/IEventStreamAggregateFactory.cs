using System;

namespace EventSourcing.Aggregates.Factories
{
    /// <summary>
    /// Creates an object of provided <see cref="Type"/> that represents an aggregate.
    /// </summary>
    public interface IEventStreamAggregateFactory
    {
        /// <summary>
        /// Creates an instance of provided <paramref name="aggregateType"/>.
        /// </summary>
        /// <param name="aggregateType">
        /// The <see cref="Type"/> to create.
        /// </param>
        /// <returns>
        /// An instance of <paramref name="aggregateType"/>.
        /// </returns>
        object Create(Type aggregateType);
    }
}
