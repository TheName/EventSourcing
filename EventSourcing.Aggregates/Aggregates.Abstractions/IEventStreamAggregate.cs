﻿using EventSourcing.Aggregates.Builders;
using EventSourcing.Aggregates.Factories;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates
{
    /// <summary>
    /// An example of an interface representing an event stream aggregate.
    /// </summary>
    /// <remarks>
    /// The aggregates do NOT need to implement this interface.
    /// However if you choose to NOT use this interface - you need to register your own implementations of <see cref="IEventStreamAggregateBuilder"/> and <see cref="IEventStreamAggregateFactory"/>.
    /// </remarks>
    public interface IEventStreamAggregate
    {
        /// <summary>
        /// Represents an instance of <see cref="PublishableEventStream"/> that can be used when publishing this aggregate's changes.
        /// </summary>
        PublishableEventStream PublishableEventStream { get; }

        /// <summary>
        /// Replays events encapsulated in the provided <paramref name="eventStream"/>.
        /// </summary>
        /// <param name="eventStream">
        /// An instance of <see cref="EventStream"/> that encapsulates already stored events.
        /// </param>
        void ReplayEventStream(EventStream eventStream);
    }
}
