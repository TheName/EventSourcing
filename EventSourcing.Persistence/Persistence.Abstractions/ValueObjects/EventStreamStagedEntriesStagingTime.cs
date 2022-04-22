using System;

namespace EventSourcing.Persistence.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream entries staging time value object.
    /// <remarks>
    /// The staging time, specified in UTC, of event stream entries.
    /// </remarks>
    /// </summary>
    public class EventStreamStagedEntriesStagingTime
    {
        /// <summary>
        /// The actual value of staging time
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Returns a new instance of <see cref="EventStreamStagedEntriesStagingTime"/> representing current moment in time.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="EventStreamStagedEntriesStagingTime"/> representing current moment in time.
        /// </returns>
        public static EventStreamStagedEntriesStagingTime Now() => DateTime.UtcNow;

        private EventStreamStagedEntriesStagingTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamStagedEntriesStagingTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamStagedEntriesStagingTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));
                
            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(EventStreamStagedEntriesStagingTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamStagedEntriesStagingTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="stagingTime">
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(EventStreamStagedEntriesStagingTime stagingTime) => stagingTime.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </summary>
        /// <param name="stagingTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </returns>
        public static implicit operator EventStreamStagedEntriesStagingTime(DateTime stagingTime) => new EventStreamStagedEntriesStagingTime(stagingTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="stagingTime">
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </param>
        /// <param name="otherStagingTime">
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="stagingTime"/> and <paramref name="otherStagingTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamStagedEntriesStagingTime stagingTime, EventStreamStagedEntriesStagingTime otherStagingTime) =>
            Equals(stagingTime, otherStagingTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="stagingTime">
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </param>
        /// <param name="otherStagingTime">
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="stagingTime"/> and <paramref name="otherStagingTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamStagedEntriesStagingTime stagingTime, EventStreamStagedEntriesStagingTime otherStagingTime) =>
            !(stagingTime == otherStagingTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamStagedEntriesStagingTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString("O");
    }
}