using System;
using EventSourcing.Aggregates.Helpers;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates
{
    /// <summary>
    /// A basic implementation of <see cref="IEventStreamAggregate"/>.
    /// </summary>
    public abstract class BaseEventStreamAggregate : IEventStreamAggregate
    {
        /// <summary>
        /// Represents an appendable event stream.
        /// </summary>
        protected AppendableEventStream AppendableEventStream { get; private set; }

        /// <summary>
        /// Gets aggregate's event stream's id.
        /// </summary>
        protected EventStreamId EventStreamId => AppendableEventStream.StreamId;

        /// <summary>
        /// Defines if in case of missing handler methods for event types an exception should be thrown.
        /// </summary>
        protected virtual bool ShouldIgnoreMissingHandlers { get; } = true;

        PublishableEventStream IEventStreamAggregate.PublishableEventStream => new PublishableEventStream(AppendableEventStream);

        /// <summary>
        /// Creates an instance of <see cref="BaseEventStreamAggregate"/> with an empty stream and random <see cref="EventStreamId"/>.
        /// </summary>
        protected BaseEventStreamAggregate() : this(EventStreamId.NewEventStreamId())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="BaseEventStreamAggregate"/> with an empty stream and provided <see cref="EventStreamId"/>.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        protected BaseEventStreamAggregate(EventStreamId streamId)
        {
            AppendableEventStream = new AppendableEventStream(EventStream.NewEventStream(streamId));
        }

        void IEventStreamAggregate.ReplayEventStream(EventStream eventStream)
        {
            AppendableEventStream = new AppendableEventStream(eventStream);
            foreach (var eventWithMetadata in eventStream.EventsWithMetadata)
            {
                ReplaySingleEvent(eventWithMetadata);
            }
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
        protected void AppendEvent(object @event)
        {
            var appendedEvent = AppendableEventStream.AppendEventWithMetadata(@event);
            ReplaySingleEvent(appendedEvent);
        }

        /// <summary>
        /// Replays provided event with metadata by invoking method that accepts event type or event type and event metadata.
        /// </summary>
        protected void ReplaySingleEvent(EventStreamEventWithMetadata eventWithMetadata)
        {
            BaseEventStreamAggregateMethodInvoker.Invoke(this, eventWithMetadata, ShouldIgnoreMissingHandlers);
        }
    }
}
