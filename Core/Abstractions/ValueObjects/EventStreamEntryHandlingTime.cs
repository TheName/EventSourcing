using System;

namespace EventSourcing.ValueObjects
{
    /// <summary>
    /// The event stream entry handling time value object.
    /// <remarks>
    /// The handling time, specified in UTC, of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryHandlingTime
    {
        /// <summary>
        /// The handling time, specified in UTC, of event stream entry.
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Returns a new instance of <see cref="EventStreamEntryHandlingTime"/> representing current moment in time.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="EventStreamEntryHandlingTime"/> representing current moment in time.
        /// </returns>
        public static EventStreamEntryHandlingTime Now() => DateTime.UtcNow;

        private EventStreamEntryHandlingTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEntryHandlingTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEntryHandlingTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));

            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamEntryHandlingTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntryHandlingTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="handlingTime">
        /// The <see cref="EventStreamEntryHandlingTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(EventStreamEntryHandlingTime handlingTime) => handlingTime.Value;

        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="EventStreamEntryHandlingTime"/>.
        /// </summary>
        /// <param name="handlingTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryHandlingTime"/>.
        /// </returns>
        public static implicit operator EventStreamEntryHandlingTime(DateTime handlingTime) => new EventStreamEntryHandlingTime(handlingTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="handlingTime">
        /// The <see cref="EventStreamEntryHandlingTime"/>.
        /// </param>
        /// <param name="otherHandlingTime">
        /// The <see cref="EventStreamEntryHandlingTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="handlingTime"/> and <paramref name="otherHandlingTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryHandlingTime handlingTime, EventStreamEntryHandlingTime otherHandlingTime) =>
            Equals(handlingTime, otherHandlingTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="handlingTime">
        /// The <see cref="EventStreamEntryHandlingTime"/>.
        /// </param>
        /// <param name="otherHandlingTime">
        /// The <see cref="EventStreamEntryHandlingTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="handlingTime"/> and <paramref name="otherHandlingTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryHandlingTime handlingTime, EventStreamEntryHandlingTime otherHandlingTime) =>
            !(handlingTime == otherHandlingTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryHandlingTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString("O");
    }
}
