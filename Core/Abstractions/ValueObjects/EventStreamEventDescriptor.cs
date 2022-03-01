using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// Represents a serialized event with type information. 
    /// </summary>
    public class EventStreamEventDescriptor
    {
        /// <summary>
        /// The serialized <see cref="EventStreamEventContent"/> of the event.
        /// </summary>
        public EventStreamEventContent EventContent { get; }

        /// <summary>
        /// The <see cref="SerializationFormat"/> used to serialize the <see cref="EventContent"/>.
        /// </summary>
        public SerializationFormat EventContentSerializationFormat { get; }

        /// <summary>
        /// The <see cref="EventStreamEventTypeIdentifier"/> of the event.
        /// </summary>
        public EventStreamEventTypeIdentifier EventTypeIdentifier { get; }

        /// <summary>
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/> of the <see cref="EventTypeIdentifier"/>.
        /// </summary>
        public EventStreamEventTypeIdentifierFormat EventTypeIdentifierFormat { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEventDescriptor"/> class.
        /// </summary>
        /// <param name="eventContent">
        /// The serialized <see cref="EventStreamEventContent"/> of the event.
        /// </param>
        /// <param name="eventContentSerializationFormat">
        /// The <see cref="SerializationFormat"/>.
        /// </param>
        /// <param name="eventTypeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/> of the event.
        /// </param>
        /// <param name="eventTypeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/> of the <paramref name="eventTypeIdentifier"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamEventDescriptor(
            EventStreamEventContent eventContent,
            SerializationFormat eventContentSerializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            EventContent = eventContent ?? throw new ArgumentNullException(nameof(eventContent));
            EventContentSerializationFormat = eventContentSerializationFormat ?? throw new ArgumentNullException(nameof(eventContentSerializationFormat));
            EventTypeIdentifier = eventTypeIdentifier ?? throw new ArgumentNullException(nameof(eventTypeIdentifier));
            EventTypeIdentifierFormat = eventTypeIdentifierFormat ?? throw new ArgumentNullException(nameof(eventTypeIdentifierFormat));
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
            $"Event Type Identifier: {EventTypeIdentifier}, Event Type Identifier Format: {EventTypeIdentifierFormat}, Event Content Serialization Format: {EventContentSerializationFormat}, Event Content: {EventContent}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return EventContentSerializationFormat;
            yield return EventTypeIdentifier;
            yield return EventTypeIdentifierFormat;
            yield return EventContent;
        }
    }
}