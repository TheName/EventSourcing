using System;
using System.Globalization;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream event creation time value object.
    /// <remarks>
    /// The creation time, specified in UTC, of event stream event.
    /// </remarks>
    /// </summary>
    public class EventStreamEventCreationTime
    {
        private DateTime Value { get; }

        private EventStreamEventCreationTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEventCreationTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEventCreationTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));
                
            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEventCreationTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventCreationTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(EventStreamEventCreationTime creationTime) => creationTime.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="EventStreamEventCreationTime"/>.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </returns>
        public static implicit operator EventStreamEventCreationTime(DateTime creationTime) => new EventStreamEventCreationTime(creationTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEntryCreationTime">
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </param>
        /// <param name="otherEventStreamEntryCreationTime">
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryCreationTime"/> and <paramref name="otherEventStreamEntryCreationTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventCreationTime eventStreamEntryCreationTime, EventStreamEventCreationTime otherEventStreamEntryCreationTime) =>
            Equals(eventStreamEntryCreationTime, otherEventStreamEntryCreationTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEntryCreationTime">
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </param>
        /// <param name="otherEventStreamEntryCreationTime">
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryCreationTime"/> and <paramref name="otherEventStreamEntryCreationTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventCreationTime eventStreamEntryCreationTime, EventStreamEventCreationTime otherEventStreamEntryCreationTime) =>
            !(eventStreamEntryCreationTime == otherEventStreamEntryCreationTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventCreationTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString(CultureInfo.InvariantCulture);
    }
}