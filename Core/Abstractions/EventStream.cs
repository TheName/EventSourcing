using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents an event stream.
    /// </summary>
    public class EventStream
    {
        private readonly List<EventStreamEventWithMetadata> _eventsWithMetadataToAppend;
        private EventStreamEntrySequence _maxSequence;
        
        /// <summary>
        /// Creates a new instance of <see cref="EventStream"/> initialized with a new <see cref="EventStreamId"/> and empty <see cref="EventStreamEntries"/>.
        /// </summary>
        /// <param name="streamId">
        /// Optional. The <see cref="EventStreamId"/> that should be used when creating this <see cref="EventStream"/>. If not provided, a new one will be created.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="EventStream"/> with the provided <paramref name="streamId"/> or a new <see cref="EventStreamId"/> and an empty collection of entries.
        /// </returns>
        public static EventStream NewEventStream(EventStreamId streamId = null) =>
            new EventStream(streamId ?? EventStreamId.NewEventStreamId(), new List<EventStreamEventWithMetadata>());
        
        /// <summary>
        /// The <see cref="EventStreamId"/> that identifies given stream of events.
        /// </summary>
        public EventStreamId StreamId { get; }
        
        /// <summary>
        /// The <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> already persisted in the stream of events.
        /// </summary>
        public IReadOnlyList<EventStreamEventWithMetadata> EventsWithMetadata { get; }

        /// <summary>
        /// Gets the <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> that is to be appended to the stream of events.
        /// </summary>
        public IReadOnlyCollection<EventStreamEventWithMetadata> EventsWithMetadataToAppend => _eventsWithMetadataToAppend.AsReadOnly();

        /// <summary>
        /// The value of <see cref="EventStreamEntrySequence"/> that should be provided in the next appended <see cref="EventsWithMetadata"/>.
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
        /// Initializes a new instance of the <see cref="EventStream"/> class.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> that identifies the stream of events.
        /// </param>
        /// <param name="eventsWithMetadata">
        /// The <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> that is already stored in the stream of events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="streamId"/> or <paramref name="eventsWithMetadata"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when provided <paramref name="streamId"/> does not match <see cref="EventStreamId"/> assigned to <paramref name="eventsWithMetadata"/>.
        /// </exception>
        /// <exception cref="InvalidEventStreamEntrySequenceException">
        /// Thrown when provided <paramref name="eventsWithMetadata"/> has minimum <see cref="EventStreamEntrySequence"/> different than 0. 
        /// </exception>
        public EventStream(
            EventStreamId streamId,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            EventsWithMetadata = eventsWithMetadata?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(eventsWithMetadata));
            _eventsWithMetadataToAppend = new List<EventStreamEventWithMetadata>();
            _maxSequence = 0;
            
            if (EventsWithMetadata.Count == 0)
            {
                return;
            }

            var minimumSequence = EventsWithMetadata[0].EventMetadata.EntrySequence;
            if (minimumSequence != 0)
            {
                throw InvalidEventStreamEntrySequenceException.New(0, minimumSequence, nameof(eventsWithMetadata));
            }
            
            _maxSequence = minimumSequence;
            
            for (var i = 1; i < EventsWithMetadata.Count; i++)
            {
                if (EventsWithMetadata[i].EventMetadata.EntrySequence != _maxSequence + 1)
                {
                    throw InvalidEventStreamEntrySequenceException.New(
                        _maxSequence + 1,
                        EventsWithMetadata[i].EventMetadata.EntrySequence,
                        $"{nameof(eventsWithMetadata)} have to be ordered increasingly by sequence and sequence has to increase by one.",
                        nameof(eventsWithMetadata));
                }

                if (EventsWithMetadata[i].EventMetadata.StreamId != streamId)
                {
                    throw InvalidEventStreamIdException.New(
                        streamId,
                        EventsWithMetadata[i].EventMetadata.StreamId,
                        $"All items of {nameof(eventsWithMetadata)} have to have same stream id.",
                        nameof(eventsWithMetadata));
                }

                _maxSequence = EventsWithMetadata[i].EventMetadata.EntrySequence;
            }
        }

        /// <summary>
        /// Appends provided <paramref name="eventsWithMetadata"/> to this instance of the stream of entries.
        /// </summary>
        /// <param name="eventsWithMetadata">
        /// The <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> that should be appended to the stream of entries.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="eventsWithMetadata"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when at least one of provided <paramref name="eventsWithMetadata"/> has <see cref="EventStreamId"/> that does not match <see cref="StreamId"/>.
        /// </exception>
        /// <exception cref="InvalidEventStreamEntrySequenceException">
        /// Thrown when at least one of provided <paramref name="eventsWithMetadata"/> has <see cref="EventStreamEntrySequence"/> other than the expected <see cref="NextSequence"/>.
        /// </exception>
        public void AppendEventsWithMetadata(IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            foreach (var eventStreamEvent in eventsWithMetadata ?? throw new ArgumentNullException(nameof(eventsWithMetadata)))
            {
                AppendEventWithMetadata(eventStreamEvent);
            }
        }

        private void AppendEventWithMetadata(EventStreamEventWithMetadata eventWithMetadata)
        {
            if (eventWithMetadata.EventMetadata.StreamId != StreamId)
            {
                throw InvalidEventStreamIdException.New(StreamId, eventWithMetadata.EventMetadata.StreamId, nameof(eventWithMetadata));
            }
            
            if (eventWithMetadata.EventMetadata.EntrySequence != NextSequence)
            {
                throw InvalidEventStreamEntrySequenceException.New(
                    NextSequence,
                    eventWithMetadata.EventMetadata.EntrySequence,
                    nameof(eventWithMetadata));
            }
            
            _eventsWithMetadataToAppend.Add(eventWithMetadata);
            _maxSequence = eventWithMetadata.EventMetadata.EntrySequence;
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStream">
        /// The <see cref="EventStream"/>.
        /// </param>
        /// <param name="otherEventStream">
        /// The <see cref="EventStream"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStream"/> and <paramref name="otherEventStream"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStream eventStream, EventStream otherEventStream) =>
            Equals(eventStream, otherEventStream);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStream">
        /// The <see cref="EventStream"/>.
        /// </param>
        /// <param name="otherEventStream">
        /// The <see cref="EventStream"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStream"/> and <paramref name="otherEventStream"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStream eventStream, EventStream otherEventStream) =>
            !(eventStream == otherEventStream);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStream other &&
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
            $"Event Stream ID: {StreamId}, {EventsWithMetadata}, Next Sequence: {NextSequence}, {EventsWithMetadataToAppendString()}";

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