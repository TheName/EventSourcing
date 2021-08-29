using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions
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
        /// The causation id. See <see cref="EventStreamEntryCausationId"/>.
        /// </summary>
        public EventStreamEntryCausationId CausationId { get; }
        
        /// <summary>
        /// The creation time. See <see cref="EventStreamEntryCreationTime"/>.
        /// </summary>
        public EventStreamEntryCreationTime CreationTime { get; }
        
        /// <summary>
        /// The correlation id. See <see cref="EventStreamEntryCorrelationId"/>.
        /// </summary>
        public EventStreamEntryCorrelationId CorrelationId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEntryMetadata"/> class.
        /// </summary>
        /// <param name="causationId">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
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
        /// <param name="metadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <param name="otherMetadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="metadata"/> and <paramref name="otherMetadata"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryMetadata metadata, EventStreamEntryMetadata otherMetadata) =>
            Equals(metadata, otherMetadata);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <param name="otherMetadata">
        /// The <see cref="EventStreamEntryMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="metadata"/> and <paramref name="otherMetadata"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryMetadata metadata, EventStreamEntryMetadata otherMetadata) =>
            !(metadata == otherMetadata);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryMetadata other &&
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