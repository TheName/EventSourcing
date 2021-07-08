using System;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream event id value object.
    /// <remarks>
    /// Identifies an instance of event stream event.
    /// </remarks>
    /// </summary>
    public class EventStreamEventId
    {
        private Guid Value { get; }

        private EventStreamEventId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEventId)} cannot be an empty guid.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamEventId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamEventId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamEventId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventId"/>.
        /// </returns>
        public static implicit operator EventStreamEventId(Guid id) => new EventStreamEventId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEventId">
        /// The <see cref="EventStreamEventId"/>.
        /// </param>
        /// <param name="otherEventStreamEventId">
        /// The <see cref="EventStreamEventId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEventId"/> and <paramref name="otherEventStreamEventId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventId eventStreamEventId, EventStreamEventId otherEventStreamEventId) =>
            Equals(eventStreamEventId, otherEventStreamEventId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEventId">
        /// The <see cref="EventStreamEventId"/>.
        /// </param>
        /// <param name="otherEventStreamEventId">
        /// The <see cref="EventStreamEventId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEventId"/> and <paramref name="otherEventStreamEventId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventId eventStreamEventId, EventStreamEventId otherEventStreamEventId) =>
            !(eventStreamEventId == otherEventStreamEventId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}