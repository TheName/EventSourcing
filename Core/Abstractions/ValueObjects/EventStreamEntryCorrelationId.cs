using System;
using System.Threading;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream entry correlation id.
    /// <remarks>
    /// Identifies the group of actions that caused the entry to be stored.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryCorrelationId
    {
        private static readonly AsyncLocal<EventStreamEntryCorrelationId> AsyncLocalCorrelationId = new AsyncLocal<EventStreamEntryCorrelationId>();
        
        /// <summary>
        /// Creates a new instance of <see cref="EventStreamEntryCorrelationId"/> initialized with a random <see cref="Guid"/>.
        /// </summary>
        public static EventStreamEntryCorrelationId NewEventStreamEntryCorrelationId() => new EventStreamEntryCorrelationId(Guid.NewGuid());

        /// <summary>
        /// Gets or sets the current async local value of <see cref="EventStreamEntryCorrelationId"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// When trying to set a new value of <see cref="Current"/> without clearing the previous one.
        /// </exception>
        public static EventStreamEntryCorrelationId Current
        {
            get
            {
                if (AsyncLocalCorrelationId.Value == null)
                {
                    AsyncLocalCorrelationId.Value = NewEventStreamEntryCorrelationId();
                }
                
                return AsyncLocalCorrelationId.Value;
            }
            set
            {
                if (AsyncLocalCorrelationId.Value != null && AsyncLocalCorrelationId.Value != value)
                {
                    throw new InvalidOperationException("CorrelationId is already set for this execution context.");
                }

                if (AsyncLocalCorrelationId.Value == value)
                {
                    return;
                }
            
                AsyncLocalCorrelationId.Value = value;
            }
        }
        
        /// <summary>
        /// The actual value of correlation id
        /// </summary>
        public Guid Value { get; }

        private EventStreamEntryCorrelationId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEntryCorrelationId)} cannot be empty guid.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntryCorrelationId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamEntryCorrelationId correlationId) => correlationId.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamEntryCorrelationId"/>.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </returns>
        public static implicit operator EventStreamEntryCorrelationId(Guid correlationId) => new EventStreamEntryCorrelationId(correlationId);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <param name="otherCorrelationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="correlationId"/> and <paramref name="otherCorrelationId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryCorrelationId correlationId, EventStreamEntryCorrelationId otherCorrelationId) =>
            Equals(correlationId, otherCorrelationId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <param name="otherCorrelationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="correlationId"/> and <paramref name="otherCorrelationId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryCorrelationId correlationId, EventStreamEntryCorrelationId otherCorrelationId) =>
            !(correlationId == otherCorrelationId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryCorrelationId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}