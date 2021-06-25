using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The causation id.
    /// <remarks>
    /// Represents a single event stream entry id that caused the entry to be stored.
    /// In a case when the entry was the first one to be stored due to action represented by correlation id, this value can be equal to <see cref="CorrelationId"/>.
    /// </remarks>
    /// </summary>
    public class CausationId
    {
        private Guid Value { get; }

        private CausationId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(CausationId)} cannot be empty guid.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="CausationId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="CausationId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(CausationId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="CausationId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="CausationId"/>.
        /// </returns>
        public static implicit operator CausationId(Guid id) => new CausationId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="CausationId"/>.
        /// </param>
        /// <param name="otherCausationId">
        /// The <see cref="CausationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="causationId"/> and <paramref name="otherCausationId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(CausationId causationId, CausationId otherCausationId) =>
            Equals(causationId, otherCausationId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="CausationId"/>.
        /// </param>
        /// <param name="otherCausationId">
        /// The <see cref="CausationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="causationId"/> and <paramref name="otherCausationId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(CausationId causationId, CausationId otherCausationId) =>
            !(causationId == otherCausationId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is CausationId other &&
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