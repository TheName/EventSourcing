using System;
using System.Threading;

namespace EventSourcing.ValueObjects
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
        private static readonly AsyncLocal<EventStreamEntryCausationId> AsyncLocalCausationId = new AsyncLocal<EventStreamEntryCausationId>();

        /// <summary>
        /// Gets the current async local value of <see cref="EventStreamEntryCausationId"/>.
        /// </summary>
        public static EventStreamEntryCausationId CurrentOrNone => AsyncLocalCausationId.Value;

        /// <summary>
        /// Gets or sets the current async local value of <see cref="EventStreamEntryCausationId"/>.
        /// <remarks>
        /// When trying to get the current value and it's not set - the code automatically sets the value to new causation id
        /// </remarks>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// When trying to set a new value of <see cref="Current"/> without clearing the previous one.
        /// </exception>
        public static EventStreamEntryCausationId Current
        {
            get
            {
                if (AsyncLocalCausationId.Value == null)
                {
                    AsyncLocalCausationId.Value = (Guid) EventStreamEntryCorrelationId.Current;
                }

                return AsyncLocalCausationId.Value;
            }
            set
            {
                if (AsyncLocalCausationId.Value != null && AsyncLocalCausationId.Value != value)
                {
                    throw new InvalidOperationException("CorrelationId is already set for this execution context.");
                }

                if (AsyncLocalCausationId.Value == value)
                {
                    return;
                }

                AsyncLocalCausationId.Value = value;
            }
        }

        /// <summary>
        /// The actual value of causation id
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EventStreamEntryCausationId"/>
        /// </summary>
        /// <param name="value">
        /// The <see cref="Guid"/> representing <see cref="EventStreamEntryCausationId"/>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is an empty guid
        /// </exception>
        public EventStreamEntryCausationId(Guid value)
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
        /// Implicit operator that converts the <see cref="EventStreamEntryId"/> to <see cref="EventStreamEntryCausationId"/>.
        /// </summary>
        /// <param name="entryId">
        /// The <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </returns>
        public static implicit operator EventStreamEntryCausationId(EventStreamEntryId entryId) => new EventStreamEntryCausationId(entryId);

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
