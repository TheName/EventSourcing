using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The correlation id.
    /// <remarks>
    /// Identifies the group of actions that caused the entry to be stored.
    /// </remarks>
    /// </summary>
    public class CorrelationId
    {
        private Guid Value { get; }

        private CorrelationId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(CorrelationId)} cannot be empty guid.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="CorrelationId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="CorrelationId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(CorrelationId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="CorrelationId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="CorrelationId"/>.
        /// </returns>
        public static implicit operator CorrelationId(Guid id) => new CorrelationId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="CorrelationId"/>.
        /// </param>
        /// <param name="otherCorrelationId">
        /// The <see cref="CorrelationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="correlationId"/> and <paramref name="otherCorrelationId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(CorrelationId correlationId, CorrelationId otherCorrelationId) =>
            Equals(correlationId, otherCorrelationId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="CorrelationId"/>.
        /// </param>
        /// <param name="otherCorrelationId">
        /// The <see cref="CorrelationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="correlationId"/> and <paramref name="otherCorrelationId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(CorrelationId correlationId, CorrelationId otherCorrelationId) =>
            !(correlationId == otherCorrelationId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is CorrelationId other &&
            other.GetHashCode() == GetHashCode();

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}