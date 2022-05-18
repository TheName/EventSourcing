using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The forgettable payload id value object.
    /// <remarks>
    /// Identifies an instance of forgettable payload object.
    /// </remarks>
    /// </summary>
    public class ForgettablePayloadId
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayloadId"/> initialized with a random <see cref="Guid"/>.
        /// </summary>
        public static ForgettablePayloadId NewForgettablePayloadId() => new ForgettablePayloadId(Guid.NewGuid());
        
        /// <summary>
        /// The actual value of forgettable payload id
        /// </summary>
        public Guid Value { get; }

        private ForgettablePayloadId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(ForgettablePayloadId)} cannot be empty guid.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="ForgettablePayloadId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(ForgettablePayloadId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="ForgettablePayloadId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadId"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadId(Guid id) => new ForgettablePayloadId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgettablePayloadId">
        /// The <see cref="ForgettablePayloadId"/>.
        /// </param>
        /// <param name="otherForgettablePayloadId">
        /// The <see cref="ForgettablePayloadId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadId"/> and <paramref name="otherForgettablePayloadId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadId forgettablePayloadId, ForgettablePayloadId otherForgettablePayloadId) =>
            Equals(forgettablePayloadId, otherForgettablePayloadId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgettablePayloadId">
        /// The <see cref="ForgettablePayloadId"/>.
        /// </param>
        /// <param name="otherForgettablePayloadId">
        /// The <see cref="ForgettablePayloadId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadId"/> and <paramref name="otherForgettablePayloadId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadId forgettablePayloadId, ForgettablePayloadId otherForgettablePayloadId) =>
            !(forgettablePayloadId == otherForgettablePayloadId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}