using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream entry id value object.
    /// <remarks>
    /// Identifies an instance of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryId
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventStreamEntryId"/> initialized with a random <see cref="Guid"/>.
        /// </summary>
        public static EventStreamEntryId NewEventStreamEntryId() => new EventStreamEntryId(Guid.NewGuid());
        
        /// <summary>
        /// The actual value of entry id
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EventStreamEntryId"/>
        /// </summary>
        /// <param name="value">
        /// The <see cref="Guid"/> representing <see cref="EventStreamEntryId"/>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is an empty guid
        /// </exception>
        public EventStreamEntryId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEntryId)} cannot be an empty guid.", nameof(value));
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
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}