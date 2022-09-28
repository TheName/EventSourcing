using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// Represents actual event's object type.
    /// </summary>
    public class EventStreamEventTypeIdentifier
    {
        /// <summary>
        /// The actual value of event type identifier
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EventStreamEventTypeIdentifier"/>
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> representing <see cref="EventStreamEventTypeIdentifier"/>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is null or whitespace
        /// </exception>
        public EventStreamEventTypeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(EventStreamEventTypeIdentifier)} cannot be null or whitespace.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventTypeIdentifier"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="typeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(EventStreamEventTypeIdentifier typeIdentifier) => typeIdentifier.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="EventStreamEventTypeIdentifier"/>.
        /// </summary>
        /// <param name="typeIdentifier">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventTypeIdentifier"/>.
        /// </returns>
        public static implicit operator EventStreamEventTypeIdentifier(string typeIdentifier) => new EventStreamEventTypeIdentifier(typeIdentifier);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEventTypeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/>.
        /// </param>
        /// <param name="otherEventStreamEventTypeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEventTypeIdentifier"/> and <paramref name="otherEventStreamEventTypeIdentifier"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventTypeIdentifier eventStreamEventTypeIdentifier, EventStreamEventTypeIdentifier otherEventStreamEventTypeIdentifier) =>
            Equals(eventStreamEventTypeIdentifier, otherEventStreamEventTypeIdentifier);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEventTypeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/>.
        /// </param>
        /// <param name="otherEventStreamEventTypeIdentifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEventTypeIdentifier"/> and <paramref name="otherEventStreamEventTypeIdentifier"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventTypeIdentifier eventStreamEventTypeIdentifier, EventStreamEventTypeIdentifier otherEventStreamEventTypeIdentifier) =>
            !(eventStreamEventTypeIdentifier == otherEventStreamEventTypeIdentifier);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventTypeIdentifier other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}