using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.Persistence.DataTransferObjects;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream entry metadata value object.
    /// <remarks>
    /// The metadata of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryMetadata
    {
        /// <summary>
        /// The causation id. See <see cref="CausationId"/>.
        /// </summary>
        public EventStreamEntryCausationId CausationId { get; }
        
        /// <summary>
        /// The causation id. See <see cref="EventStreamEntryCreationTime"/>.
        /// </summary>
        public EventStreamEntryCreationTime CreationTime { get; }
        
        /// <summary>
        /// The correlation id. See <see cref="CorrelationId"/>.
        /// </summary>
        public EventStreamEntryCorrelationId CorrelationId { get; }

        public EventStreamEntryMetadata(
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
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
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <param name="otherEventStreamEntryMetadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryMetadata"/> and <paramref name="otherEventStreamEntryMetadata"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryMetadata eventStreamEntryMetadata, EventStreamEntryMetadata otherEventStreamEntryMetadata) =>
            Equals(eventStreamEntryMetadata, otherEventStreamEntryMetadata);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEntryMetadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <param name="otherEventStreamEntryMetadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryMetadata"/> and <paramref name="otherEventStreamEntryMetadata"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryMetadata eventStreamEntryMetadata, EventStreamEntryMetadata otherEventStreamEntryMetadata) =>
            !(eventStreamEntryMetadata == otherEventStreamEntryMetadata);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryMetadata other &&
            other.GetHashCode() == GetHashCode();

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Aggregate(17, (current, property) => current * 23 * property.GetHashCode());
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