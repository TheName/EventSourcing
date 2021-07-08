using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents a single event in an event stream.
    /// </summary>
    public class EventStreamEvent
    {
        /// <summary>
        /// The <see cref="EventStreamId"/> that this event belongs to.
        /// </summary>
        public EventStreamId StreamId { get; }
        
        /// <summary>
        /// The <see cref="EventStreamId"/> assigned to this event.
        /// </summary>
        public EventStreamEventId EventId { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEventSequence"/> this event has in the event stream it belongs to.
        /// </summary>
        public EventStreamEventSequence EventSequence { get; }
        
        /// <summary>
        /// The actual event object.
        /// </summary>
        public object Event { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEventMetadata"/> related to this event.
        /// </summary>
        public EventStreamEventMetadata EventMetadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEvent"/> class.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> that this event belongs to.
        /// </param>
        /// <param name="eventId">
        /// The <see cref="EventStreamId"/> assigned to this event.
        /// </param>
        /// <param name="eventSequence">
        /// The <see cref="EventStreamEventSequence"/> this event has in the event stream it belongs to.
        /// </param>
        /// <param name="event">
        /// The actual event object.
        /// </param>
        /// <param name="eventMetadata">
        /// The <see cref="EventStreamEventMetadata"/> related to this event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamEvent(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
            EventSequence = eventSequence ?? throw new ArgumentNullException(nameof(eventSequence));
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            EventMetadata = eventMetadata ?? throw new ArgumentNullException(nameof(eventMetadata));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="event">
        /// The <see cref="EventStreamEvent"/>.
        /// </param>
        /// <param name="otherEvent">
        /// The <see cref="EventStreamEvent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="event"/> and <paramref name="otherEvent"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEvent @event, EventStreamEvent otherEvent) =>
            Equals(@event, otherEvent);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="event">
        /// The <see cref="EventStreamEvent"/>.
        /// </param>
        /// <param name="otherEvent">
        /// The <see cref="EventStreamEvent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="event"/> and <paramref name="otherEvent"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEvent @event, EventStreamEvent otherEvent) =>
            !(@event == otherEvent);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEvent other &&
            other.GetPropertiesForHashCode().SequenceEqual(GetPropertiesForHashCode());

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Select(o => o.GetHashCode())
                    .Where(i => i != 0)
                    .Aggregate(17, (current, hashCode) => current * 23 * hashCode);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Event Stream ID: {StreamId}, Event ID: {EventId}, Event Sequence: {EventSequence}, Event: {Event}, EventMetadata: {EventMetadata}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return EventId;
            yield return EventSequence;
            yield return Event;
            yield return EventMetadata;
        }
    }
}