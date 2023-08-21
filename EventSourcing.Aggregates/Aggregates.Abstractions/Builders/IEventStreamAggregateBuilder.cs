using System;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates.Builders
{
    /// <summary>
    /// Builds an aggregate of provided <see cref="Type"/> and replayed <see cref="EventStream"/>.
    /// </summary>
    public interface IEventStreamAggregateBuilder
    {
        /// <summary>
        /// Builds an object of provided <paramref name="aggregateType"/> and replays provided <paramref name="eventStream"/> on it.
        /// </summary>
        /// <param name="aggregateType">
        /// The aggregate's <see cref="Type"/>.
        /// </param>
        /// <param name="eventStream">
        /// The <see cref="EventStream"/> to replay on the aggregate.
        /// </param>
        /// <returns>
        /// An instance of type <paramref name="aggregateType"/> with replayed events from <paramref name="eventStream"/>.
        /// </returns>
        object Build(Type aggregateType, EventStream eventStream);
    }
}
