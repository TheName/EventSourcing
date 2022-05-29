using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The forgettable payload last modified time value object.
    /// <remarks>
    /// The last modified time, specified in UTC, of forgettable payload.
    /// </remarks>
    /// </summary>
    public class ForgettablePayloadLastModifiedTime
    {
        /// <summary>
        /// The actual value of last modified time
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Returns a new instance of <see cref="ForgettablePayloadLastModifiedTime"/> representing current moment in time.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="ForgettablePayloadLastModifiedTime"/> representing current moment in time.
        /// </returns>
        public static ForgettablePayloadLastModifiedTime Now() => DateTime.UtcNow;

        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayloadLastModifiedTime"/>
        /// </summary>
        /// <param name="value">
        /// The actual value of last modified time
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if provided value is DateTime.MinValue or DateTime.MaxValue or Kind is different than Utc 
        /// </exception>
        public ForgettablePayloadLastModifiedTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettablePayloadLastModifiedTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettablePayloadLastModifiedTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));
                
            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettablePayloadLastModifiedTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadLastModifiedTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="lastModifiedTime">
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(ForgettablePayloadLastModifiedTime lastModifiedTime) => lastModifiedTime.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </summary>
        /// <param name="lastModifiedTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadLastModifiedTime(DateTime lastModifiedTime) => new ForgettablePayloadLastModifiedTime(lastModifiedTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="lastModifiedTime">
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </param>
        /// <param name="otherLastModifiedTime">
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="lastModifiedTime"/> and <paramref name="otherLastModifiedTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadLastModifiedTime lastModifiedTime, ForgettablePayloadLastModifiedTime otherLastModifiedTime) =>
            Equals(lastModifiedTime, otherLastModifiedTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="lastModifiedTime">
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </param>
        /// <param name="otherLastModifiedTime">
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="lastModifiedTime"/> and <paramref name="otherLastModifiedTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadLastModifiedTime lastModifiedTime, ForgettablePayloadLastModifiedTime otherLastModifiedTime) =>
            !(lastModifiedTime == otherLastModifiedTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadLastModifiedTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString("O");
    }
}