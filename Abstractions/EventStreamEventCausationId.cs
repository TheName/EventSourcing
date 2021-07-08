using System;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream event causation id.
    /// <remarks>
    /// Represents a single event id that caused this event to be created.
    /// In a case when the entry was the first one to be stored due to action represented by correlation id, this value can be equal to <see cref="EventStreamEventCorrelationId"/>.
    /// </remarks>
    /// </summary>
    public class EventStreamEventCausationId
    {
        private Guid Value { get; }

        private EventStreamEventCausationId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEventCausationId)} cannot be empty guid.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventCausationId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamEventCausationId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamEventCausationId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </returns>
        public static implicit operator EventStreamEventCausationId(Guid id) => new EventStreamEventCausationId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </param>
        /// <param name="otherCausationId">
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="causationId"/> and <paramref name="otherCausationId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventCausationId causationId, EventStreamEventCausationId otherCausationId) =>
            Equals(causationId, otherCausationId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </param>
        /// <param name="otherCausationId">
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="causationId"/> and <paramref name="otherCausationId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventCausationId causationId, EventStreamEventCausationId otherCausationId) =>
            !(causationId == otherCausationId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventCausationId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}