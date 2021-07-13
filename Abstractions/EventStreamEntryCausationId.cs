using System;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream entry causation id.
    /// <remarks>
    /// Represents a single entry id that caused this entry to be created.
    /// In a case when the entry was the first one to be stored due to action represented by correlation id, this value can be equal to <see cref="EventStreamEntryCorrelationId"/>.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryCausationId
    {
        private Guid Value { get; }

        private EventStreamEntryCausationId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEntryCausationId)} cannot be empty guid.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntryCausationId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamEntryCausationId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamEntryCausationId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </returns>
        public static implicit operator EventStreamEntryCausationId(Guid id) => new EventStreamEntryCausationId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <param name="otherCausationId">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="causationId"/> and <paramref name="otherCausationId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryCausationId causationId, EventStreamEntryCausationId otherCausationId) =>
            Equals(causationId, otherCausationId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <param name="otherCausationId">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="causationId"/> and <paramref name="otherCausationId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryCausationId causationId, EventStreamEntryCausationId otherCausationId) =>
            !(causationId == otherCausationId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryCausationId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}