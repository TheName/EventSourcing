using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents a single entry in an event stream.
    /// </summary>
    public class EventStreamEntry
    {
        /// <summary>
        /// The <see cref="EventStreamId"/> that this event belongs to.
        /// </summary>
        public EventStreamId StreamId { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEntryId"/> assigned to this entry.
        /// </summary>
        public EventStreamEntryId EntryId { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEntrySequence"/> this entry has in the event stream it belongs to.
        /// </summary>
        public EventStreamEntrySequence EntrySequence { get; }

        /// <summary>
        /// The <see cref="EventStreamEventDescriptor"/> that describes event from this entry.
        /// </summary>
        public EventStreamEventDescriptor EventDescriptor { get; }
        
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
        /// Initializes a new instance of the <see cref="EventStreamEntry"/> class.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> that this entry belongs to.
        /// </param>
        /// <param name="entryId">
        /// The <see cref="EventStreamEntryId"/> assigned to this entry.
        /// </param>
        /// <param name="entrySequence">
        /// The <see cref="EventStreamEntrySequence"/> this entry has in the event stream it belongs to.
        /// </param>
        /// <param name="eventDescriptor">
        /// The <see cref="EventStreamEventDescriptor"/> that's describing this event.
        /// </param>
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
        public EventStreamEntry(
            EventStreamId streamId,
            EventStreamEntryId entryId,
            EventStreamEntrySequence entrySequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            EntryId = entryId ?? throw new ArgumentNullException(nameof(entryId));
            EntrySequence = entrySequence ?? throw new ArgumentNullException(nameof(entrySequence));
            EventDescriptor = eventDescriptor ?? throw new ArgumentNullException(nameof(eventDescriptor));
            CausationId = causationId ?? throw new ArgumentNullException(nameof(causationId));
            CreationTime = creationTime ?? throw new ArgumentNullException(nameof(creationTime));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="event">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="otherEvent">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="event"/> and <paramref name="otherEvent"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntry @event, EventStreamEntry otherEvent) =>
            Equals(@event, otherEvent);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="event">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="otherEvent">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="event"/> and <paramref name="otherEvent"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntry @event, EventStreamEntry otherEvent) =>
            !(@event == otherEvent);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntry other &&
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
            $"Event Stream ID: {StreamId}, Entry ID: {EntryId}, Entry Sequence: {EntrySequence}, Event Descriptor: {EventDescriptor}, Causation ID: {CausationId}, Creation Time: {CreationTime}, Correlation ID: {CorrelationId}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return EntryId;
            yield return EntrySequence;
            yield return EventDescriptor;
            yield return CausationId;
            yield return CreationTime;
            yield return CorrelationId;
        }
    }
}