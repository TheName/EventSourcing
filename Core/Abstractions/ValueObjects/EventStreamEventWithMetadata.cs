using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// Represents an event with metadata.
    /// </summary>
    /// <remarks>
    /// Please keep in mind that provided event needs to be a value object as well in order for the comparison to return correct results.
    /// </remarks>
    public class EventStreamEventWithMetadata
    {
        /// <summary>
        /// The <see cref="object"/> representing an event.
        /// </summary>
        public object Event { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEventMetadata"/> representing event's metadata.
        /// </summary>
        public EventStreamEventMetadata EventMetadata { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEventWithMetadata"/> class.
        /// </summary>
        /// <param name="event">
        /// The <see cref="object"/> representing an event.
        /// </param>
        /// <param name="eventMetadata">
        /// The <see cref="EventStreamEventMetadata"/> representing event's metadata.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamEventWithMetadata(
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            EventMetadata = eventMetadata ?? throw new ArgumentNullException(nameof(eventMetadata));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventWithMetadata">
        /// The <see cref="EventStreamEventWithMetadata"/>.
        /// </param>
        /// <param name="otherEventWithMetadata">
        /// The <see cref="EventStreamEventWithMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventWithMetadata"/> and <paramref name="otherEventWithMetadata"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventWithMetadata eventWithMetadata, EventStreamEventWithMetadata otherEventWithMetadata) =>
            Equals(eventWithMetadata, otherEventWithMetadata);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventWithMetadata">
        /// The <see cref="EventStreamEventWithMetadata"/>.
        /// </param>
        /// <param name="otherEventWithMetadata">
        /// The <see cref="EventStreamEventWithMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventWithMetadata"/> and <paramref name="otherEventWithMetadata"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventWithMetadata eventWithMetadata, EventStreamEventWithMetadata otherEventWithMetadata) =>
            !(eventWithMetadata == otherEventWithMetadata);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventWithMetadata other &&
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
        public override string ToString() =>
            $"Event: {Event}, Event Metadata: {EventMetadata}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return Event;
            yield return EventMetadata;
        }
    }
}