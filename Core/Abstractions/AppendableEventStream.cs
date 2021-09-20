using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents an stream of events with metadata that can be modified by appending new events.
    /// </summary>
    public class AppendableEventStream
    {
        private readonly List<EventStreamEventWithMetadata> _eventsWithMetadataToAppend;
        private EventStreamEntrySequence _maxSequence;

        /// <summary>
        /// The <see cref="EventStreamId"/> that identifies given stream of events.
        /// </summary>
        public EventStreamId StreamId { get; }

        /// <summary>
        /// The <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> already persisted in the stream of events.
        /// </summary>
        public IReadOnlyList<EventStreamEventWithMetadata> EventsWithMetadata { get; }

        /// <summary>
        /// Gets the <see cref="IReadOnlyCollection{EventStreamEventWithMetadata}"/> that is to be appended to the stream of events.
        /// </summary>
        public IReadOnlyCollection<EventStreamEventWithMetadata> EventsWithMetadataToAppend => _eventsWithMetadataToAppend.AsReadOnly();

        /// <summary>
        /// The value of <see cref="EventStreamEntrySequence"/> that should be provided in the next appended <see cref="EventStreamEventWithMetadata"/>.
        /// </summary>
        public EventStreamEntrySequence NextSequence
        {
            get
            {
                if (_maxSequence == 0 && EventsWithMetadata.Count == 0 && EventsWithMetadataToAppend.Count == 0)
                {
                    return 0;
                }
                
                return _maxSequence + 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppendableEventStream"/> class.
        /// </summary>
        /// <param name="eventStream">
        /// The read-only <see cref="EventStream"/> of events already persisted in the event store.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="eventStream"/> is null.
        /// </exception>
        public AppendableEventStream(EventStream eventStream)
        {
            if (eventStream == null)
            {
                throw new ArgumentNullException(nameof(eventStream));
            }
            
            StreamId = eventStream.StreamId;
            EventsWithMetadata = eventStream.EventsWithMetadata;
            _maxSequence = eventStream.MaxSequence;
            _eventsWithMetadataToAppend = new List<EventStreamEventWithMetadata>();
        }

        /// <summary>
        /// Appends <see cref="EventStreamEventWithMetadata"/> to the stream.
        /// </summary>
        /// <param name="event">
        /// The event itself.
        /// </param>
        /// <param name="entryId">
        /// The <see cref="EventStreamEntryId"/>. In case null is provided, a new entry is generated.
        /// </param>
        /// <param name="causationId">
        /// The <see cref="EventStreamEntryCausationId"/>. In case null is provided, current CausationId is used.
        /// </param>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>. In case null is provided, current time is used.
        /// </param>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>. In case null is provided, current CorrelationId is used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided arguments is null.
        /// </exception>
        public EventStreamEventWithMetadata AppendEventWithMetadata(
            object @event,
            EventStreamEntryId entryId = null,
            EventStreamEntryCausationId causationId = null,
            EventStreamEntryCreationTime creationTime = null,
            EventStreamEntryCorrelationId correlationId = null)
        {
            var eventWithMetadataToAppend = new EventStreamEventWithMetadata(
                @event,
                new EventStreamEventMetadata(
                    StreamId,
                    entryId ?? EventStreamEntryId.NewEventStreamEntryId(),
                    NextSequence,
                    causationId ?? EventStreamEntryCausationId.Current,
                    creationTime ?? EventStreamEntryCreationTime.Now(),
                    correlationId ?? EventStreamEntryCorrelationId.Current));
            
            _eventsWithMetadataToAppend.Add(eventWithMetadataToAppend);
            _maxSequence = NextSequence;
            return eventWithMetadataToAppend;
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStream">
        /// The <see cref="AppendableEventStream"/>.
        /// </param>
        /// <param name="otherEventStream">
        /// The <see cref="AppendableEventStream"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStream"/> and <paramref name="otherEventStream"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(AppendableEventStream eventStream, AppendableEventStream otherEventStream) =>
            Equals(eventStream, otherEventStream);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStream">
        /// The <see cref="AppendableEventStream"/>.
        /// </param>
        /// <param name="otherEventStream">
        /// The <see cref="AppendableEventStream"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStream"/> and <paramref name="otherEventStream"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(AppendableEventStream eventStream, AppendableEventStream otherEventStream) =>
            !(eventStream == otherEventStream);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is AppendableEventStream other &&
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
            $"Event Stream ID: {StreamId}, {EventsWithMetadataString()}, Next Sequence: {NextSequence}, {EventsWithMetadataToAppendString()}";

        private string EventsWithMetadataString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("EventsWithMetadata: ");
            foreach (var eventWithMetadata in EventsWithMetadata)
            {
                stringBuilder.Append($"\n\t{eventWithMetadata}");
            }

            return stringBuilder.ToString();
        }

        private string EventsWithMetadataToAppendString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("EventsWithMetadata to append: ");
            foreach (var eventWithMetadata in _eventsWithMetadataToAppend)
            {
                stringBuilder.Append($"\n\t{eventWithMetadata}");
            }

            return stringBuilder.ToString();
        }

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return _maxSequence;
            foreach (var eventStreamEventWithMetadata in EventsWithMetadata)
            {
                yield return eventStreamEventWithMetadata;
            }
            foreach (var eventWithMetadata in _eventsWithMetadataToAppend)
            {
                yield return eventWithMetadata;
            }
        }
    }
}