using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// Represents <see cref="EventStreamEventTypeIdentifier"/>'s format
    /// </summary>
    public class EventStreamEventTypeIdentifierFormat
    {
        /// <summary>
        /// The ClassName <see cref="EventStreamEventTypeIdentifier"/>'s format.
        /// </summary>
        public static EventStreamEventTypeIdentifierFormat ClassName { get; } = new EventStreamEventTypeIdentifierFormat(nameof(ClassName));
        
        /// <summary>
        /// The actual value of event type identifier format
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EventStreamEventTypeIdentifierFormat"/>
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> representing <see cref="EventStreamEventTypeIdentifierFormat"/>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is null or whitespace
        /// </exception>
        public EventStreamEventTypeIdentifierFormat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(EventStreamEventTypeIdentifierFormat)} cannot be null or whitespace.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventTypeIdentifierFormat"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="typeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(EventStreamEventTypeIdentifierFormat typeIdentifierFormat) => typeIdentifierFormat.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </summary>
        /// <param name="typeIdentifierFormat">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </returns>
        public static implicit operator EventStreamEventTypeIdentifierFormat(string typeIdentifierFormat) => new EventStreamEventTypeIdentifierFormat(typeIdentifierFormat);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEventTypeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </param>
        /// <param name="otherEventStreamEventTypeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEventTypeIdentifierFormat"/> and <paramref name="otherEventStreamEventTypeIdentifierFormat"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventTypeIdentifierFormat eventStreamEventTypeIdentifierFormat, EventStreamEventTypeIdentifierFormat otherEventStreamEventTypeIdentifierFormat) =>
            Equals(eventStreamEventTypeIdentifierFormat, otherEventStreamEventTypeIdentifierFormat);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEventTypeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </param>
        /// <param name="otherEventStreamEventTypeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEventTypeIdentifierFormat"/> and <paramref name="otherEventStreamEventTypeIdentifierFormat"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventTypeIdentifierFormat eventStreamEventTypeIdentifierFormat, EventStreamEventTypeIdentifierFormat otherEventStreamEventTypeIdentifierFormat) =>
            !(eventStreamEventTypeIdentifierFormat == otherEventStreamEventTypeIdentifierFormat);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventTypeIdentifierFormat other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}