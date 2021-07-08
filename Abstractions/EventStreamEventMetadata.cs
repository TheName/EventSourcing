using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream event metadata value object.
    /// <remarks>
    /// The metadata of event stream event.
    /// </remarks>
    /// </summary>
    public class EventStreamEventMetadata
    {
        /// <summary>
        /// The causation id. See <see cref="CausationId"/>.
        /// </summary>
        public EventStreamEventCausationId CausationId { get; }
        
        /// <summary>
        /// The creation time. See <see cref="EventStreamEventCreationTime"/>.
        /// </summary>
        public EventStreamEventCreationTime CreationTime { get; }
        
        /// <summary>
        /// The correlation id. See <see cref="CorrelationId"/>.
        /// </summary>
        public EventStreamEventCorrelationId CorrelationId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEventMetadata"/> class.
        /// </summary>
        /// <param name="causationId">
        /// The causation id. See <see cref="CausationId"/>.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id. See <see cref="CorrelationId"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamEventMetadata(
            EventStreamEventCausationId causationId,
            EventStreamEventCorrelationId correlationId)
            : this(
                causationId,
                DateTime.UtcNow,
                correlationId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEventMetadata"/> class.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="EventStreamEventCausationId"/>.
        /// </param>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEventCreationTime"/>.
        /// </param>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEventCorrelationId"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamEventMetadata(
            EventStreamEventCausationId causationId,
            EventStreamEventCreationTime creationTime,
            EventStreamEventCorrelationId correlationId)
        {
            CausationId = causationId ?? throw new ArgumentNullException(nameof(causationId));
            CreationTime = creationTime ?? throw new ArgumentNullException(nameof(creationTime));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEntryMetadata">
        /// The <see cref="EventStreamEventMetadata"/>.
        /// </param>
        /// <param name="otherEventStreamEntryMetadata">
        /// The <see cref="EventStreamEventMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryMetadata"/> and <paramref name="otherEventStreamEntryMetadata"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventMetadata eventStreamEntryMetadata, EventStreamEventMetadata otherEventStreamEntryMetadata) =>
            Equals(eventStreamEntryMetadata, otherEventStreamEntryMetadata);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEntryMetadata">
        /// The <see cref="EventStreamEventMetadata"/>.
        /// </param>
        /// <param name="otherEventStreamEntryMetadata">
        /// The <see cref="EventStreamEventMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryMetadata"/> and <paramref name="otherEventStreamEntryMetadata"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventMetadata eventStreamEntryMetadata, EventStreamEventMetadata otherEventStreamEntryMetadata) =>
            !(eventStreamEntryMetadata == otherEventStreamEntryMetadata);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventMetadata other &&
            other.GetPropertiesForHashCode().SequenceEqual(GetPropertiesForHashCode());

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Select(o => o.GetHashCode())
                    .Where(i => i != 0)
                    .Aggregate(17, (current, hashCode) => current * 23 * hashCode);
            }
        }

        /// <inheritdoc />
        public override string ToString()=>
            $"Causation ID: {CausationId}, Creation Time: {CreationTime}, Correlation ID: {CorrelationId}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return CausationId;
            yield return CreationTime;
            yield return CorrelationId;
        }
    }
}