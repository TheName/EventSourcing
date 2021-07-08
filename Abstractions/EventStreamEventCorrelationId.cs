using System;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream event correlation id.
    /// <remarks>
    /// Identifies the group of actions that caused the event to be stored.
    /// </remarks>
    /// </summary>
    public class EventStreamEventCorrelationId
    {
        private Guid Value { get; }

        private EventStreamEventCorrelationId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(EventStreamEventCorrelationId)} cannot be empty guid.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventCorrelationId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static implicit operator Guid(EventStreamEventCorrelationId id) => id.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="Guid"/> to <see cref="EventStreamEventCorrelationId"/>.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </returns>
        public static implicit operator EventStreamEventCorrelationId(Guid id) => new EventStreamEventCorrelationId(id);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </param>
        /// <param name="otherCorrelationId">
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="correlationId"/> and <paramref name="otherCorrelationId"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventCorrelationId correlationId, EventStreamEventCorrelationId otherCorrelationId) =>
            Equals(correlationId, otherCorrelationId);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </param>
        /// <param name="otherCorrelationId">
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="correlationId"/> and <paramref name="otherCorrelationId"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventCorrelationId correlationId, EventStreamEventCorrelationId otherCorrelationId) =>
            !(correlationId == otherCorrelationId);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventCorrelationId other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}