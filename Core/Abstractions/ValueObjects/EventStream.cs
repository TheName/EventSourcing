using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Exceptions;

namespace EventSourcing.ValueObjects
{
    /// <summary>
    /// Represents a read-only event stream.
    /// </summary>
    public class EventStream
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventStream"/> initialized with a new <see cref="EventStreamId"/> and empty <see cref="IEnumerable{EventStreamEventWithMetadata}"/>.
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
        /// The <see cref="EventStreamEntrySequence"/> that represents the max sequence of an event stored in this event stream.
        /// </summary>
        public EventStreamEntrySequence MaxSequence { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream"/> class.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> that identifies the stream of events.
        /// </param>
        /// <param name="eventsWithMetadata">
        /// The <see cref="IEnumerable{EventStreamEventWithMetadata}"/> that is already stored in the stream of events.
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

            if (EventsWithMetadata.Count == 0)
            {
                MaxSequence = 0;
                return;
            }

            foreach (var eventWithMetadata in EventsWithMetadata)
            {
                if (eventWithMetadata.EventMetadata.StreamId != streamId)
                {
                    throw InvalidEventStreamIdException.New(
                        streamId,
                        eventWithMetadata.EventMetadata.StreamId,
                        $"All items of {nameof(eventsWithMetadata)} have to have same stream id.",
                        nameof(eventsWithMetadata));
                }

                var expectedSequence = MaxSequence == null ? 0 : MaxSequence + 1;
                if (eventWithMetadata.EventMetadata.EntrySequence != expectedSequence)
                {
                    throw InvalidEventStreamEntrySequenceException.New(
                        expectedSequence,
                        eventWithMetadata.EventMetadata.EntrySequence,
                        $"{nameof(eventsWithMetadata)} have to be ordered increasingly by sequence and sequence has to increase by one.",
                        nameof(eventsWithMetadata));
                }

                MaxSequence = eventWithMetadata.EventMetadata.EntrySequence;
            }
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
        public override string ToString() => $"Event Stream ID: {StreamId}, Max Sequence: {MaxSequence}, {EventsWithMetadataString()}";

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

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return MaxSequence;
            foreach (var eventStreamEventWithMetadata in EventsWithMetadata)
            {
                yield return eventStreamEventWithMetadata;
            }
        }
    }
}
