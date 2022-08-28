using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The forgetting payload time value object.
    /// <remarks>
    /// The forgetting payload time, specified in UTC, of the forgettable payload.
    /// </remarks>
    /// </summary>
    public class ForgettingPayloadTime
    {
        /// <summary>
        /// The actual value of forgetting time
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Returns a new instance of <see cref="ForgettingPayloadTime"/> representing current moment in time.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="ForgettingPayloadTime"/> representing current moment in time.
        /// </returns>
        public static ForgettingPayloadTime Now() => DateTime.UtcNow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettingPayloadTime"/> class.
        /// </summary>
        /// <param name="value">
        /// The actual value of forgetting time
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is DateTime.MinValue or DateTime.MaxValue or Kind is different than UTC
        /// </exception>
        public ForgettingPayloadTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettingPayloadTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettingPayloadTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));
                
            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettingPayloadTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettingPayloadTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="forgettingTime">
        /// The <see cref="ForgettingPayloadTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(ForgettingPayloadTime forgettingTime) => forgettingTime.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="ForgettingPayloadTime"/>.
        /// </summary>
        /// <param name="forgettingTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettingPayloadTime"/>.
        /// </returns>
        public static implicit operator ForgettingPayloadTime(DateTime forgettingTime) => new ForgettingPayloadTime(forgettingTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgettingTime">
        /// The <see cref="ForgettingPayloadTime"/>.
        /// </param>
        /// <param name="otherForgettingTime">
        /// The <see cref="ForgettingPayloadTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettingTime"/> and <paramref name="otherForgettingTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettingPayloadTime forgettingTime, ForgettingPayloadTime otherForgettingTime) =>
            Equals(forgettingTime, otherForgettingTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgettingTime">
        /// The <see cref="ForgettingPayloadTime"/>.
        /// </param>
        /// <param name="otherForgettingTime">
        /// The <see cref="ForgettingPayloadTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettingTime"/> and <paramref name="otherForgettingTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettingPayloadTime forgettingTime, ForgettingPayloadTime otherForgettingTime) =>
            !(forgettingTime == otherForgettingTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettingPayloadTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString("O");
    }
}