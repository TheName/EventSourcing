using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// Describes the entity that requested payload to be forgotten
    /// </summary>
    public class ForgettingPayloadRequestedBy
    {
        /// <summary>
        /// The max length of reason text.
        /// </summary>
        public static readonly int MaxLength = 500;

        /// <summary>
        /// The Requested By Data Owner entity
        /// </summary>
        public static ForgettingPayloadRequestedBy DataOwner = "Data Owner";

        /// <summary>
        /// The Requested By Unclaimed Forgettable Payload Cleanup Job entity
        /// </summary>
        public static ForgettingPayloadRequestedBy UnclaimedForgettablePayloadCleanupJob = "Unclaimed Forgettable Payload Cleanup Job";
        
        /// <summary>
        /// The actual value of the description.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ForgettingPayloadRequestedBy"/>
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
        public ForgettingPayloadRequestedBy(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(ForgettingPayloadRequestedBy)} cannot be null or whitespace.", nameof(value));
            }

            if (value.Length > MaxLength)
            {
                throw new ArgumentException($"{nameof(ForgettingPayloadRequestedBy)} cannot be longer than {MaxLength}",
                    nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettingPayloadRequestedBy"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="requestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(ForgettingPayloadRequestedBy requestedBy) => requestedBy.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="ForgettingPayloadRequestedBy"/>.
        /// </summary>
        /// <param name="requestedBy">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </returns>
        public static implicit operator ForgettingPayloadRequestedBy(string requestedBy) => new ForgettingPayloadRequestedBy(requestedBy);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="requestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </param>
        /// <param name="otherRequestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="requestedBy"/> and <paramref name="otherRequestedBy"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettingPayloadRequestedBy requestedBy, ForgettingPayloadRequestedBy otherRequestedBy) =>
            Equals(requestedBy, otherRequestedBy);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="requestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </param>
        /// <param name="otherRequestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="requestedBy"/> and <paramref name="otherRequestedBy"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettingPayloadRequestedBy requestedBy, ForgettingPayloadRequestedBy otherRequestedBy) =>
            !(requestedBy == otherRequestedBy);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettingPayloadRequestedBy other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}