using System;

namespace EventSourcing.Abstractions.Persistence.DataTransferObjects
{
    /// <summary>
    /// The event stream entry id value object.
    /// <remarks>
    /// Identifies an instance of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryId
    {
        private Guid Value { get; }

        private EventStreamEntryId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEntryId)} cannot be empty guid.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntryId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamEntryId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamEntryId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryId"/>.
        /// </returns>
        public static implicit operator EventStreamEntryId(Guid id) => new EventStreamEntryId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <param name="otherEventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryId"/> and <paramref name="otherEventStreamEntryId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryId eventStreamEntryId, EventStreamEntryId otherEventStreamEntryId) =>
            Equals(eventStreamEntryId, otherEventStreamEntryId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <param name="otherEventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryId"/> and <paramref name="otherEventStreamEntryId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryId eventStreamEntryId, EventStreamEntryId otherEventStreamEntryId) =>
            !(eventStreamEntryId == otherEventStreamEntryId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryId other &&
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