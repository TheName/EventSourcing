using System;
using System.Globalization;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream entry creation time value object.
    /// <remarks>
    /// The creation time, specified in UTC, of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryCreationTime
    {
        private DateTime Value { get; }

        private EventStreamEntryCreationTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEntryCreationTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEntryCreationTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));
                
            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEntryCreationTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntryCreationTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(EventStreamEntryCreationTime creationTime) => creationTime.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="EventStreamEntryCreationTime"/>.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </returns>
        public static implicit operator EventStreamEntryCreationTime(DateTime creationTime) => new EventStreamEntryCreationTime(creationTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <param name="otherCreationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="creationTime"/> and <paramref name="otherCreationTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryCreationTime creationTime, EventStreamEntryCreationTime otherCreationTime) =>
            Equals(creationTime, otherCreationTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <param name="otherCreationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="creationTime"/> and <paramref name="otherCreationTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryCreationTime creationTime, EventStreamEntryCreationTime otherCreationTime) =>
            !(creationTime == otherCreationTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryCreationTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString(CultureInfo.InvariantCulture);
    }
}