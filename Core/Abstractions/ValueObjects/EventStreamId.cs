using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream id value object.
    /// <remarks>
    /// Identifies an instance of event stream.
    /// </remarks>
    /// </summary>
    public class EventStreamId
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventStreamId"/> initialized with a random <see cref="Guid"/>.
        /// </summary>
        public static EventStreamId NewEventStreamId() => new EventStreamId(Guid.NewGuid());
        
        private Guid Value { get; }

        private EventStreamId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamId)} cannot be empty guid.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamId"/>.
        /// </returns>
        public static implicit operator EventStreamId(Guid id) => new EventStreamId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="otherEventStreamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamId"/> and <paramref name="otherEventStreamId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamId eventStreamId, EventStreamId otherEventStreamId) =>
            Equals(eventStreamId, otherEventStreamId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="otherEventStreamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamId"/> and <paramref name="otherEventStreamId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamId eventStreamId, EventStreamId otherEventStreamId) =>
            !(eventStreamId == otherEventStreamId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}