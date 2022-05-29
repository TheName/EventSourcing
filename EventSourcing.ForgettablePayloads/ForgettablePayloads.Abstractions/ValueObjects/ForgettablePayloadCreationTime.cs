using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The forgettable payload creation time value object.
    /// <remarks>
    /// The creation time, specified in UTC, of forgettable payload.
    /// </remarks>
    /// </summary>
    public class ForgettablePayloadCreationTime
    {
        /// <summary>
        /// The actual value of creation time
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Returns a new instance of <see cref="ForgettablePayloadCreationTime"/> representing current moment in time.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="ForgettablePayloadCreationTime"/> representing current moment in time.
        /// </returns>
        public static ForgettablePayloadCreationTime Now() => DateTime.UtcNow;

        private ForgettablePayloadCreationTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettablePayloadCreationTime)} must have a different value than {DateTime.MinValue}.",
                    nameof(value));
            }

            if (value == DateTime.MaxValue)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettablePayloadCreationTime)} must have a different value than {DateTime.MaxValue}.",
                    nameof(value));
                
            }

            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    $"{nameof(ForgettablePayloadCreationTime)} must have {DateTimeKind.Utc} date time kind.",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadCreationTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="ForgettablePayloadCreationTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static implicit operator DateTime(ForgettablePayloadCreationTime creationTime) => creationTime.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="DateTime"/> to <see cref="ForgettablePayloadCreationTime"/>.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadCreationTime"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadCreationTime(DateTime creationTime) => new ForgettablePayloadCreationTime(creationTime);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="ForgettablePayloadCreationTime"/>.
        /// </param>
        /// <param name="otherCreationTime">
        /// The <see cref="ForgettablePayloadCreationTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="creationTime"/> and <paramref name="otherCreationTime"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadCreationTime creationTime, ForgettablePayloadCreationTime otherCreationTime) =>
            Equals(creationTime, otherCreationTime);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="creationTime">
        /// The <see cref="ForgettablePayloadCreationTime"/>.
        /// </param>
        /// <param name="otherCreationTime">
        /// The <see cref="ForgettablePayloadCreationTime"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="creationTime"/> and <paramref name="otherCreationTime"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadCreationTime creationTime, ForgettablePayloadCreationTime otherCreationTime) =>
            !(creationTime == otherCreationTime);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadCreationTime other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString("O");
    }
}