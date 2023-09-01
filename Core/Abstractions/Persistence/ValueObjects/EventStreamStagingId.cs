using System;

namespace EventSourcing.Persistence.ValueObjects
{
    /// <summary>
    /// The event stream staging id value object.
    /// <remarks>
    /// Identifies an instance of event stream staging entries.
    /// </remarks>
    /// </summary>
    public class EventStreamStagingId
    {
        /// <summary>
        /// The actual value of staging id
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EventStreamStagingId"/> initialized with a random <see cref="Guid"/>.
        /// </summary>
        public static EventStreamStagingId NewEventStreamStagingId() => new EventStreamStagingId(Guid.NewGuid());

        private EventStreamStagingId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamStagingId)} cannot be an empty guid.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamStagingId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamStagingId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamStagingId id) => id.Value;

        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamStagingId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamStagingId"/>.
        /// </returns>
        public static implicit operator EventStreamStagingId(Guid id) => new EventStreamStagingId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamStagingId">
        /// The <see cref="EventStreamStagingId"/>.
        /// </param>
        /// <param name="otherEventStreamStagingId">
        /// The <see cref="EventStreamStagingId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamStagingId"/> and <paramref name="otherEventStreamStagingId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamStagingId eventStreamStagingId, EventStreamStagingId otherEventStreamStagingId) =>
            Equals(eventStreamStagingId, otherEventStreamStagingId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamStagingId">
        /// The <see cref="EventStreamStagingId"/>.
        /// </param>
        /// <param name="otherEventStreamStagingId">
        /// The <see cref="EventStreamStagingId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamStagingId"/> and <paramref name="otherEventStreamStagingId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamStagingId eventStreamStagingId, EventStreamStagingId otherEventStreamStagingId) =>
            !(eventStreamStagingId == otherEventStreamStagingId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamStagingId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}
