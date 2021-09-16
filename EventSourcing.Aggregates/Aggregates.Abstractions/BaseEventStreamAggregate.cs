using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions.Helpers;

namespace EventSourcing.Aggregates.Abstractions
{
    /// <summary>
    /// A basic implementation of <see cref="IEventStreamAggregate"/>.
    /// </summary>
    public abstract class BaseEventStreamAggregate : IEventStreamAggregate
    {
        /// <summary>
        /// Represents an appendable event stream.
        /// </summary>
        protected internal AppendableEventStream AppendableEventStream { get; private set; } =
            new AppendableEventStream(EventStream.NewEventStream());

        /// <summary>
        /// Defines if in case of missing handler methods for event types an exception should be thrown.
        /// </summary>
        protected internal virtual bool ShouldIgnoreMissingHandlers { get; } = false;

        /// <inheritdoc />
        public PublishableEventStream PublishableEventStream => new PublishableEventStream(AppendableEventStream);

        /// <inheritdoc />
        public void Replay(EventStream eventStream)
        {
            foreach (var eventWithMetadata in eventStream.EventsWithMetadata)
            {
                ReplayEvent(eventWithMetadata);
            }
            
            AppendableEventStream = new AppendableEventStream(eventStream);
        }

        /// <summary>
        /// Appends provided <paramref name="event"/> to the in-memory stream.
        /// </summary>
        /// <param name="event">
        /// The <see cref="object"/> representing an event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="event"/> is null.
        /// </exception>
        protected internal void Append(object @event)
        {
            var appendedEvent = AppendableEventStream.AppendEventWithMetadata(@event);
            ReplayEvent(appendedEvent);
        }

        /// <summary>
        /// Replays provided event with metadata by invoking method that accepts event type or event type and event metadata.
        /// </summary>
        protected internal void ReplayEvent(EventStreamEventWithMetadata eventWithMetadata)
        {
            BaseEventStreamAggregateMethodInvoker.Invoke(this, eventWithMetadata);
        }
    }
}