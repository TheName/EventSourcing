using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents a single event in an event stream. The event is serialized and its type is explicitly stored. 
    /// </summary>
    public class EventStreamEventDescriptor
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
        /// The serialized <see cref="EventStreamEventContent"/> of the actual event's object.
        /// </summary>
        public EventStreamEventContent EventContent { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEventTypeIdentifier"/> of the actual event's object.
        /// </summary>
        public EventStreamEventTypeIdentifier EventTypeIdentifier { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEventMetadata"/> related to this event.
        /// </summary>
        public EventStreamEventMetadata EventMetadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEventDescriptor"/> class.
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
        /// <param name="eventContent">
        /// The serialized <see cref="EventStreamEventContent"/> of the actual event's object.
        /// </param>
        /// <param name="eventTypeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/> of the actual event's object.
        /// </param>
        /// <param name="eventMetadata">
        /// The <see cref="EventStreamEventMetadata"/> related to this event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamEventDescriptor(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
            EventSequence = eventSequence ?? throw new ArgumentNullException(nameof(eventSequence));
            EventContent = eventContent ?? throw new ArgumentNullException(nameof(eventContent));
            EventTypeIdentifier = eventTypeIdentifier ?? throw new ArgumentNullException(nameof(eventTypeIdentifier));
            EventMetadata = eventMetadata ?? throw new ArgumentNullException(nameof(eventMetadata));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventDescriptor">
        /// The <see cref="EventStreamEventDescriptor"/>.
        /// </param>
        /// <param name="otherEventDescriptor">
        /// The <see cref="EventStreamEventDescriptor"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventDescriptor"/> and <paramref name="otherEventDescriptor"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventDescriptor eventDescriptor, EventStreamEventDescriptor otherEventDescriptor) =>
            Equals(eventDescriptor, otherEventDescriptor);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventDescriptor">
        /// The <see cref="EventStreamEventDescriptor"/>.
        /// </param>
        /// <param name="otherEventDescriptor">
        /// The <see cref="EventStreamEventDescriptor"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventDescriptor"/> and <paramref name="otherEventDescriptor"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventDescriptor eventDescriptor, EventStreamEventDescriptor otherEventDescriptor) =>
            !(eventDescriptor == otherEventDescriptor);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventDescriptor other &&
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
            $"Event Stream ID: {StreamId}, Event ID: {EventId}, Event Sequence: {EventSequence}, Event Content: {EventContent}, Event Type Identifier: {EventTypeIdentifier}, EventMetadata: {EventMetadata}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return EventId;
            yield return EventSequence;
            yield return EventContent;
            yield return EventTypeIdentifier;
            yield return EventMetadata;
        }
    }
}