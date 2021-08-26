using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions.Exceptions;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents an event stream.
    /// </summary>
    public class EventStream
    {
        private readonly List<EventStreamEntry> _entriesToAppend;

        /// <summary>
        /// Creates a new instance of <see cref="EventStream"/> initialized with a new <see cref="EventStreamId"/> and empty <see cref="EventStreamEntries"/>.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="EventStream"/> with a new <see cref="EventStreamId"/> and an empty collection of entries.
        /// </returns>
        public static EventStream NewEventStream() =>
            new EventStream(EventStreamId.NewEventStreamId(), EventStreamEntries.Empty);
        
        /// <summary>
        /// The <see cref="EventStreamId"/> that identifies given stream of events.
        /// </summary>
        public EventStreamId StreamId { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEntries"/> already persisted in the stream of events.
        /// </summary>
        public EventStreamEntries Entries { get; }
        
        /// <summary>
        /// The current highest <see cref="EventStreamEntrySequence"/> of both persisted events and events to store in the stream of events.  
        /// </summary>
        public EventStreamEntrySequence CurrentSequence { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="EventStreamEntry"/> that is to be appended to the stream of events.
        /// </summary>
        public EventStreamEntries EntriesToAppend => new EventStreamEntries(_entriesToAppend);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream"/> class.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> that identifies the stream of events.
        /// </param>
        /// <param name="entries">
        /// The collection of <see cref="EventStreamEntry"/> that are already stored in the stream of events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="streamId"/> or <paramref name="entries"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when provided <paramref name="streamId"/> does not match <see cref="EventStreamId"/> assigned to <paramref name="entries"/>.
        /// </exception>
        /// <exception cref="InvalidEventStreamEntrySequenceException">
        /// Thrown when provided <paramref name="entries"/> has its `MinimumSequence` different than 0. 
        /// </exception>
        public EventStream(
            EventStreamId streamId,
            EventStreamEntries entries)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
            CurrentSequence = Entries.MaximumSequence;
            _entriesToAppend = new List<EventStreamEntry>();
            
            if (Entries.Count == 0)
            {
                return;
            }
            
            if (Entries.StreamId != StreamId)
            {
                throw InvalidEventStreamIdException.New(Entries.StreamId, StreamId, nameof(streamId));
            }

            if (Entries.MinimumSequence != 0)
            {
                throw InvalidEventStreamEntrySequenceException.New(0, Entries.MinimumSequence, nameof(entries));
            }
        }

        /// <summary>
        /// Appends provided <paramref name="entries"/> to this instance of the stream of entries.
        /// </summary>
        /// <param name="entries">
        /// The collection of <see cref="EventStreamEntry"/> that should be appended to the stream of entries.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="entries"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when at least one of provided <paramref name="entries"/> has <see cref="EventStreamId"/> that does not match <see cref="StreamId"/>.
        /// </exception>
        /// <exception cref="InvalidEventStreamEntrySequenceException">
        /// Thrown when at least one of provided <paramref name="entries"/> has <see cref="EventStreamEntrySequence"/> other than <see cref="CurrentSequence"/> + 1 (increased sequentially).
        /// </exception>
        public void AppendEntries(IEnumerable<EventStreamEntry> entries)
        {
            foreach (var eventStreamEvent in entries ?? throw new ArgumentNullException(nameof(entries)))
            {
                AppendEntry(eventStreamEvent);
            }
        }

        private void AppendEntry(EventStreamEntry entry)
        {
            if (entry.StreamId != StreamId)
            {
                throw InvalidEventStreamIdException.New(StreamId, entry.StreamId, nameof(entry));
            }

            EventStreamEntrySequence expectedSequence = CurrentSequence + 1;
            if (CurrentSequence == 0 && Entries.Count == 0 && EntriesToAppend.Count == 0)
            {
                expectedSequence = CurrentSequence;
            }
            
            if (entry.EntrySequence != expectedSequence)
            {
                throw InvalidEventStreamEntrySequenceException.New(
                    expectedSequence,
                    entry.EntrySequence,
                    nameof(entry));
            }
            
            _entriesToAppend.Add(entry);
            CurrentSequence = entry.EntrySequence;
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
            $"Event Stream ID: {StreamId}, {Entries}, Current Sequence: {CurrentSequence}, {EntriesToAppendString()}";

        private string EntriesToAppendString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Entries to append: ");
            foreach (var eventStreamEntry in _entriesToAppend)
            {
                stringBuilder.Append($"\n\t{eventStreamEntry}");
            }

            return stringBuilder.ToString();
        }

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return Entries;
            yield return CurrentSequence;
            foreach (var eventStreamEvent in _entriesToAppend)
            {
                yield return eventStreamEvent;
            }
        }
    }
}