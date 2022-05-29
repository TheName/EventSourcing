using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The reason payload was forgotten
    /// </summary>
    public class ForgettingPayloadReason
    {
        /// <summary>
        /// The max length of reason text.
        /// </summary>
        public static readonly int MaxLength = 500;

        /// <summary>
        /// Requested by data owner reason
        /// </summary>
        public static readonly ForgettingPayloadReason RequestedByDataOwner = "Requested by data owner";

        /// <summary>
        /// Requested by data owner reason
        /// </summary>
        public static ForgettingPayloadReason GetDueToBeingUnclaimedForLongerThan(TimeSpan timeoutTimeSpan)
        {
            return $"Payload was not claimed within configured timeout - {timeoutTimeSpan}";
        }
        
        /// <summary>
        /// The actual value of the reason text.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ForgettingPayloadReason"/>
        /// </summary>
        /// <param name="value">
        /// The reason text.
        /// <remarks>
        /// Please keep in mind this data is stored in forgettable payload storage after forgetting the original payload. DO NOT store data in this text that should be at some point forgotten.
        /// </remarks>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if text is null or whitespace or larger than <see cref="MaxLength"/>
        /// </exception>
        public ForgettingPayloadReason(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(ForgettingPayloadReason)} cannot be null or whitespace.", nameof(value));
            }

            if (value.Length > MaxLength)
            {
                throw new ArgumentException($"{nameof(ForgettingPayloadReason)} cannot be longer than {MaxLength}",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettingPayloadReason"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="reason">
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(ForgettingPayloadReason reason) => reason.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="ForgettingPayloadReason"/>.
        /// </summary>
        /// <param name="reason">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </returns>
        public static implicit operator ForgettingPayloadReason(string reason) => new ForgettingPayloadReason(reason);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="reason">
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </param>
        /// <param name="otherReason">
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="reason"/> and <paramref name="otherReason"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettingPayloadReason reason, ForgettingPayloadReason otherReason) =>
            Equals(reason, otherReason);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="reason">
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </param>
        /// <param name="otherReason">
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="reason"/> and <paramref name="otherReason"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettingPayloadReason reason, ForgettingPayloadReason otherReason) =>
            !(reason == otherReason);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettingPayloadReason other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}