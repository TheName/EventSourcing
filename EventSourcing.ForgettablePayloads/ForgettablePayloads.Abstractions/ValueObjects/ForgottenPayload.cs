using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// The payload that will replace original payload when forgetting original payload
    /// <remarks>
    /// Please keep in mind this data is stored in forgettable payload storage after forgetting the original payload. DO NOT store data in here that should be at some point forgotten.
    /// </remarks>
    /// </summary>
    public class ForgottenPayload
    {
        /// <summary>
        /// The time of the forgetting action
        /// </summary>
        public ForgettingPayloadTime ForgettingTime { get; }

        /// <summary>
        /// The reason why the original payload was forgotten
        /// </summary>
        public ForgettingPayloadReason ForgettingReason { get; }

        /// <summary>
        /// The entity that requested the original payload to be forgotten
        /// </summary>
        public ForgettingPayloadRequestedBy ForgettingRequestedBy { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ForgottenPayload"/>
        /// </summary>
        /// <param name="reason">
        /// The reason why the original payload was forgotten
        /// </param>
        /// <param name="requestedBy">
        /// The entity that requested the original payload to be forgotten
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ForgottenPayload"/>
        /// </returns>
        public static ForgottenPayload Create(ForgettingPayloadReason reason, ForgettingPayloadRequestedBy requestedBy)
        {
            return new ForgottenPayload(
                ForgettingPayloadTime.Now(),
                reason,
                requestedBy);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ForgottenPayload"/>
        /// </summary>
        /// <param name="forgettingTime">
        /// The time of the forgetting action
        /// </param>
        /// <param name="forgettingReason">
        /// The reason why the original payload was forgotten
        /// </param>
        /// <param name="forgettingRequestedBy">
        /// The entity that requested the original payload to be forgotten
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public ForgottenPayload(
            ForgettingPayloadTime forgettingTime,
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            ForgettingTime = forgettingTime ?? throw new ArgumentNullException(nameof(forgettingTime));
            ForgettingReason = forgettingReason ?? throw new ArgumentNullException(nameof(forgettingReason));
            ForgettingRequestedBy = forgettingRequestedBy ?? throw new ArgumentNullException(nameof(forgettingRequestedBy));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgottenPayload">
        /// The <see cref="ForgottenPayload"/>.
        /// </param>
        /// <param name="otherForgottenPayload">
        /// The <see cref="ForgottenPayload"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgottenPayload"/> and <paramref name="otherForgottenPayload"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgottenPayload forgottenPayload, ForgottenPayload otherForgottenPayload) =>
            Equals(forgottenPayload, otherForgottenPayload);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgottenPayload">
        /// The <see cref="ForgottenPayload"/>.
        /// </param>
        /// <param name="otherForgottenPayload">
        /// The <see cref="ForgottenPayload"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgottenPayload"/> and <paramref name="otherForgottenPayload"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgottenPayload forgottenPayload, ForgottenPayload otherForgottenPayload) =>
            !(forgottenPayload == otherForgottenPayload);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgottenPayload other &&
            other.GetPropertiesForHashCode().SequenceEqual(GetPropertiesForHashCode());

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Select(o => o.GetHashCode())
                    .Where(i => i != 0)
                    .Aggregate(17, (current, hashCode) => current * 23 * hashCode);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Forgetting Time: {ForgettingTime}, Forgetting Reason: {ForgettingReason}, Forgetting Requested By: {ForgettingRequestedBy}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return ForgettingTime;
            yield return ForgettingReason;
            yield return ForgettingRequestedBy;
        }
    }
}
