using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.Exceptions;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents an event stream.
    /// </summary>
    public class EventStream
    {
        private readonly List<EventStreamEvent> _eventsToAppend;

        /// <summary>
        /// Creates a new instance of <see cref="EventStream"/> initialized with a new <see cref="EventStreamId"/> and empty <see cref="EventStreamEvents"/>.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="EventStream"/> with a new <see cref="EventStreamId"/> and an empty collection of events.
        /// </returns>
        public static EventStream NewEventStream() =>
            new EventStream(EventStreamId.NewEventStreamId(), EventStreamEvents.Empty);
        
        /// <summary>
        /// The <see cref="EventStreamId"/> that identifies given stream of events.
        /// </summary>
        public EventStreamId StreamId { get; }
        
        /// <summary>
        /// The <see cref="EventStreamEvents"/> already persisted in the stream of events.
        /// </summary>
        public EventStreamEvents Events { get; }
        
        /// <summary>
        /// The current highest <see cref="EventStreamEventSequence"/> of both persisted events and events to store in the stream of events.  
        /// </summary>
        public EventStreamEventSequence CurrentSequence { get; private set; }
        
        /// <summary>
        /// Gets the collection of <see cref="EventStreamEvent"/> that is to be appended to the stream of events.
        /// </summary>
        public IReadOnlyList<EventStreamEvent> EventsToAppend => _eventsToAppend.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream"/> class.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> that identifies the stream of events.
        /// </param>
        /// <param name="events">
        /// The collection of <see cref="EventStreamEvent"/> that are already stored in the stream of events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="streamId"/> or <paramref name="events"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when provided <paramref name="streamId"/> does not match <see cref="EventStreamId"/> assigned to <paramref name="events"/>.
        /// </exception>
        /// <exception cref="InvalidEventStreamEventSequenceException">
        /// Thrown when provided <paramref name="events"/> has its `MinimumSequence` different than 0. 
        /// </exception>
        public EventStream(
            EventStreamId streamId,
            EventStreamEvents events)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            CurrentSequence = Events.MaximumSequence;
            _eventsToAppend = new List<EventStreamEvent>();
            
            if (Events.Count == 0)
            {
                return;
            }
            
            if (Events.StreamId != StreamId)
            {
                throw InvalidEventStreamIdException.New(Events.StreamId, StreamId, nameof(streamId));
            }

            if (Events.MinimumSequence != 0)
            {
                throw InvalidEventStreamEventSequenceException.New(0, Events.MinimumSequence, nameof(events));
            }
        }

        /// <summary>
        /// Appends provided <paramref name="events"/> to this instance of the stream of events.
        /// </summary>
        /// <param name="events">
        /// The collection of <see cref="EventStreamEvent"/> that should be appended to the stream of events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="events"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when at least one of provided <paramref name="events"/> has <see cref="EventStreamId"/> that does not match <see cref="StreamId"/>.
        /// </exception>
        /// <exception cref="InvalidEventStreamEventSequenceException">
        /// Thrown when at least one of provided <paramref name="events"/> has <see cref="EventStreamEventSequence"/> other than <see cref="CurrentSequence"/> + 1 (increased sequentially).
        /// </exception>
        public void AppendEvents(IEnumerable<EventStreamEvent> events)
        {
            foreach (var eventStreamEvent in events ?? throw new ArgumentNullException(nameof(events)))
            {
                AppendEvent(eventStreamEvent);
            }
        }

        private void AppendEvent(EventStreamEvent @event)
        {
            if (@event.StreamId != StreamId)
            {
                throw InvalidEventStreamIdException.New(StreamId, @event.StreamId, nameof(@event));
            }

            EventStreamEventSequence expectedSequence = CurrentSequence + 1;
            if (CurrentSequence == 0 && Events.Count == 0 && EventsToAppend.Count == 0)
            {
                expectedSequence = CurrentSequence;
            }
            
            if (@event.EventSequence != expectedSequence)
            {
                throw InvalidEventStreamEventSequenceException.New(
                    expectedSequence,
                    @event.EventSequence,
                    nameof(@event));
            }
            
            _eventsToAppend.Add(@event);
            CurrentSequence = @event.EventSequence;
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
        /// True if <paramref name="event"/> and <paramref name="otherEventStream"/> are equal, false otherwise.
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
            $"Event Stream ID: {StreamId}, Current Sequence: {CurrentSequence}, Number of events to append: {_eventsToAppend.Count}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return Events;
            yield return CurrentSequence;
            foreach (var eventStreamEvent in _eventsToAppend)
            {
                yield return eventStreamEvent;
            }
        }
    }
}