using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStream_Should
    {
        [Fact]
        public void CreateNewEventStreamWithEmptyEvents_When_CallingNewEventStream()
        {
            var stream = EventStream.NewEventStream();
            Assert.NotNull(stream.StreamId);
            Assert.Empty(stream.Events);
            Assert.Equal<uint>(0, stream.CurrentSequence);
            Assert.Empty(stream.EventsToAppend);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullStreamId(
            EventStreamEvents events)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(
                null,
                events));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEvents(
            EventStreamId streamId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(
                streamId,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_CreatingWithDifferentStreamIdThanAssignedToEvents(
            EventStreamId streamId,
            EventStreamEvents events)
        {
            Assert.Throws<InvalidEventStreamIdException>(() => new EventStream(
                streamId, 
                events));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEventSequenceException_When_CreatingWithSameStreamIdAsAssignedToEvents_And_EventsSequenceDoesNotStartWithZero(
            EventStreamEvents events)
        {
            var streamId = events[0].StreamId;
            Assert.Throws<InvalidEventStreamEventSequenceException>(() => new EventStream(
                streamId,
                events));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullStreamIdAndEmptyEventsCollection(
            EventStreamId streamId)
        {
            _ = new EventStream(
                streamId,
                EventStreamEvents.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithSameStreamIdAsAssignedToEvents_And_EventsSequenceStartsWithZero(
            EventStreamEvents events)
        {
            var streamId = events[0].StreamId;
            events = new EventStreamEvents(events
                .Select((@event, i) => new EventStreamEvent(
                    @event.StreamId,
                    @event.EventId,
                    Convert.ToUInt32(i),
                    @event.Event,
                    @event.EventMetadata)));

            _ = new EventStream(
                streamId,
                events);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(EventStreamEvents events)
        {
            var streamId = events[0].StreamId;
            events = new EventStreamEvents(events
                .Select((@event, i) => new EventStreamEvent(
                    @event.StreamId,
                    @event.EventId,
                    Convert.ToUInt32(i),
                    @event.Event,
                    @event.EventMetadata)));
            var stream = new EventStream(streamId, events);

            Assert.Equal(streamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsProvidedDuringCreation_When_GettingEvents(EventStreamEvents events)
        {
            var streamId = events[0].StreamId;
            events = new EventStreamEvents(events
                .Select((@event, i) => new EventStreamEvent(
                    @event.StreamId,
                    @event.EventId,
                    Convert.ToUInt32(i),
                    @event.Event,
                    @event.EventMetadata)));
            var stream = new EventStream(streamId, events);

            Assert.Equal(new EventStreamEvents(events.AsEnumerable()), stream.Events);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsMaximumSequenceProvidedDuringCreation_When_GettingCurrentSequence(EventStreamEvents events)
        {
            var streamId = events[0].StreamId;
            events = new EventStreamEvents(events
                .Select((@event, i) => new EventStreamEvent(
                    @event.StreamId,
                    @event.EventId,
                    Convert.ToUInt32(i),
                    @event.Event,
                    @event.EventMetadata)));
            var stream = new EventStream(streamId, events);

            Assert.Equal(new EventStreamEvents(events.AsEnumerable()).MaximumSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEmptyCollection_When_GettingEventsToAppend(EventStreamEvents events)
        {
            var streamId = events[0].StreamId;
            events = new EventStreamEvents(events
                .Select((@event, i) => new EventStreamEvent(
                    @event.StreamId,
                    @event.EventId,
                    Convert.ToUInt32(i),
                    @event.Event,
                    @event.EventMetadata)));
            var stream = new EventStream(streamId, events);

            Assert.Empty(stream.EventsToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AppendingEvents_And_ProvidedEventsIsNull(
            EventStream stream)
        {
            Assert.Throws<ArgumentNullException>(() => stream.AppendEvents(null));
        }

        [Fact]
        public void Throw_ArgumentNullException_When_AppendingEvents_And_ProvidedEventsIsNull_And_EventStreamIsEmpty()
        {
            var stream = EventStream.NewEventStream();
            Assert.Throws<ArgumentNullException>(() => stream.AppendEvents(null));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEvents_And_SingleEventHasInvalidStreamId(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEvents(new[] {eventToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEvents_And_SingleEventHasInvalidStreamId_And_EventStreamIsEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEvents(new[] {eventToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEvents_And_FirstEventHasInvalidStreamId(
            EventStream stream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    i == 0 ? @event.StreamId : validStreamId,
                    @event.EventId,
                    @event.EventSequence,
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEvents_And_FirstEventHasInvalidStreamId_And_EventStreamIsEmpty(
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    i == 0 ? @event.StreamId : validStreamId,
                    @event.EventId,
                    @event.EventSequence,
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEvents_And_AtLeastOneOfEventsHasInvalidStreamId_And_AllEventsHaveCorrectSequence(
            int invalidStreamIdIndex,
            EventStream stream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            var nextSequence = stream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    i == invalidStreamIdIndex ? @event.StreamId : validStreamId,
                    @event.EventId,
                    nextSequence++,
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEvents_And_AtLeastOneOfEventsHasInvalidStreamId_And_AllEventsHaveCorrectSequence_And_EventStreamIsEmpty(
            int invalidStreamIdIndex,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint nextSequence = 0;
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    i == invalidStreamIdIndex ? @event.StreamId : validStreamId,
                    @event.EventId,
                    nextSequence++,
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEventSequenceException_When_AppendingEvents_And_SingleEventHasValidStreamIdButInvalidSequence(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            var validStreamId = stream.StreamId;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                eventToAppend.EventSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, eventToAppend.EventSequence);
            Assert.Throws<InvalidEventStreamEventSequenceException>(() => stream.AppendEvents(new[] {eventToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEventSequenceException_When_AppendingEvents_And_SingleEventHasValidStreamIdButInvalidSequence_And_EventStreamIsEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                eventToAppend.EventSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, eventToAppend.EventSequence);

            Assert.Throws<InvalidEventStreamEventSequenceException>(() => stream.AppendEvents(new[] {eventToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEventSequenceException_When_AppendingEvents_And_FirstEventHasValidStreamIdButInvalidSequence(
            EventStream stream,
            List<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    i == 0 ? validStreamId : @event.StreamId,
                    @event.EventId,
                    @event.EventSequence,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, eventsToAppend[0].EventSequence);

            Assert.Throws<InvalidEventStreamEventSequenceException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEventSequenceException_When_AppendingEvents_And_FirstEventHasValidStreamIdButInvalidSequence_And_EventStreamIsEmpty(
            List<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    i == 0 ? validStreamId : @event.StreamId,
                    @event.EventId,
                    @event.EventSequence,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, eventsToAppend[0].EventSequence);

            Assert.Throws<InvalidEventStreamEventSequenceException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamEventSequenceException_When_AppendingEvents_And_AllEventsHaveValidStreamId_And_AtLeastOneEventHasInvalidSequence(
            int invalidSequenceIndex,
            EventStream stream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            var nextSequence = stream.CurrentSequence + 1;
            var getNextSequenceFunc = new Func<int, EventStreamEvent, EventStreamEventSequence>(
                (currentIndex, @event) =>
                {
                    var currentValidSequence = nextSequence++;
                    Assert.NotEqual<uint>(@event.EventSequence, currentValidSequence);
                    return currentIndex == invalidSequenceIndex ? @event.EventSequence : currentValidSequence;
                });
            
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    getNextSequenceFunc(i, @event),
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamEventSequenceException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamEventSequenceExceptions_When_AppendingEvents_And_AllEventsHaveValidStreamId_And_AtLeastOneEventHasInvalidSequence_And_EventStreamIsEmpty(
            int invalidSequenceIndex,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextSequence = stream.CurrentSequence + 1;
            var getNextSequenceFunc = new Func<int, EventStreamEvent, EventStreamEventSequence>(
                (currentIndex, @event) =>
                {
                    var currentValidSequence = nextSequence++;
                    Assert.NotEqual<uint>(@event.EventSequence, currentValidSequence);
                    return currentIndex == invalidSequenceIndex ? @event.EventSequence : currentValidSequence;
                });
            
            eventsToAppend = eventsToAppend
                .Select((@event, i) => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    getNextSequenceFunc(i, @event),
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamEventSequenceException>(() => stream.AppendEvents(eventsToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEvents_And_SingleEventHasValidStreamIdAndValidSequence(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEvents_And_SingleEventHasValidStreamIdAndValidSequence_And_EventStreamIsEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEvents_And_AllEventsHaveValidStreamIdAndValidSequence(
            EventStream stream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata));

            stream.AppendEvents(eventsToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEvents_And_AllEventsHaveValidStreamIdAndValidSequence_And_EventStreamIsEmpty(
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata));

            stream.AppendEvents(eventsToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingSingleEvent(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingSingleEvent_And_EventStreamWasEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingEvents(
            EventStream stream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata));

            stream.AppendEvents(eventsToAppend);
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingEvents_And_EventStreamWasEmpty(
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata));

            stream.AppendEvents(eventsToAppend);
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEvents_When_GettingEventsAfterAppendingSingleEvent(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            var originalEvents = stream.Events;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
            
            Assert.Equal(originalEvents, stream.Events);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEvents_When_GettingEventsAfterAppendingSingleEvent_And_EventStreamWasEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalEvents = stream.Events;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
            
            Assert.Equal(originalEvents, stream.Events);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEvents_When_GettingEventsAfterAppendingEvents(
            EventStream stream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var originalEvents = stream.Events;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata));

            stream.AppendEvents(eventsToAppend);
            
            Assert.Equal(originalEvents, stream.Events);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEvents_When_GettingEventsAfterAppendingEvents_And_EventStreamWasEmpty(
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalEvents = stream.Events;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata));

            stream.AppendEvents(eventsToAppend);
            
            Assert.Equal(originalEvents, stream.Events);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventSequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingSingleEvent(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
            
            Assert.Equal<EventStreamEventSequence>(validSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventSequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingSingleEvent_And_EventStreamWasEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});
            
            Assert.Equal<EventStreamEventSequence>(validSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastAppendedEventSequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingEvents(
            EventStream stream,
            List<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();

            stream.AppendEvents(eventsToAppend);
            
            Assert.Equal(eventsToAppend.Last().EventSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastAppendedEventSequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingEvents_And_EventStreamWasEmpty(
            List<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();

            stream.AppendEvents(eventsToAppend);
            
            Assert.Equal(eventsToAppend.Last().EventSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventAsSingleEventToAppend_When_GettingEventsToAppendAfterAppendingSingleEvent(
            EventStream stream,
            EventStreamEvent eventToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});

            var singleEventToAppend = Assert.Single(stream.EventsToAppend);
            Assert.Equal(eventToAppend, singleEventToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventAsSingleEventToAppend_When_GettingEventsToAppendAfterAppendingSingleEvent_And_EventStreamWasEmpty(
            EventStreamEvent eventToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventToAppend = new EventStreamEvent(
                validStreamId,
                eventToAppend.EventId,
                validSequence,
                eventToAppend.Event,
                eventToAppend.EventMetadata);

            stream.AppendEvents(new[] {eventToAppend});

            var singleEventToAppend = Assert.Single(stream.EventsToAppend);
            Assert.Equal(eventToAppend, singleEventToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventsAsEventsToAppend_When_GettingEventsToAppendAfterAppendingEvents(
            EventStream stream,
            List<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();

            stream.AppendEvents(eventsToAppend);
            
            Assert.True(stream.EventsToAppend.SequenceEqual(eventsToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventsAsEventsToAppend_When_GettingEventsToAppendAfterAppendingEvents_And_EventStreamWasEmpty(
            List<EventStreamEvent> eventsToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();

            stream.AppendEvents(eventsToAppend);
            
            Assert.True(stream.EventsToAppend.SequenceEqual(eventsToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEmptyCollectionOfEvents(
            EventStreamId streamId)
        {
            var stream1 = new EventStream(
                streamId,
                EventStreamEvents.Empty);

            var stream2 = new EventStream(
                streamId,
                EventStreamEvents.Empty);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEvents(
            EventStream eventStream)
        {
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEvents_And_SameAppendedEvents(
            EventStream eventStream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            stream1.AppendEvents(eventsToAppend);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);
            
            stream2.AppendEvents(eventsToAppend);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamIdAndEmptyCollectionOfEvents(
            EventStreamId streamId1,
            EventStreamId streamId2)
        {
            var stream1 = new EventStream(
                streamId1,
                EventStreamEvents.Empty);

            var stream2 = new EventStream(
                streamId2,
                EventStreamEvents.Empty);
            
            Assert.NotEqual(stream1.GetHashCode(), stream2.GetHashCode());
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamIdAndEvents(
            EventStream eventStream1,
            EventStream eventStream2)
        {
            var stream1 = new EventStream(
                eventStream1.StreamId,
                eventStream1.Events);

            var stream2 = new EventStream(
                eventStream2.StreamId,
                eventStream2.Events);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEvents_And_OneHasAppendedEventsWhileTheOtherDoesNot(
            EventStream eventStream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            stream1.AppendEvents(eventsToAppend);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEvents_And_BothHaveDifferentNumberAppendedEvents(
            EventStream eventStream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            stream1.AppendEvents(eventsToAppend);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            stream2.AppendEvents(eventsToAppend.Take(new Random().Next(0, eventsToAppend.Count())));
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEvents_And_BothHaveDifferentAppendedEvents(
            EventStream eventStream,
            IEnumerable<EventStreamEvent> eventsToAppend1,
            IEnumerable<EventStreamEvent> eventsToAppend2)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            eventsToAppend1 = eventsToAppend1
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            eventsToAppend2 = eventsToAppend2
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            stream1.AppendEvents(eventsToAppend1);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Events);

            stream1.AppendEvents(eventsToAppend2);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStream eventStream)
        {
            var expectedValue =
                $"Event Stream ID: {eventStream.StreamId}, Current Sequence: {eventStream.CurrentSequence}, Number of events to append: {eventStream.EventsToAppend.Count}";
            
            Assert.Equal(expectedValue, eventStream.ToString());
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToStringWithAppendedEvents(
            EventStream eventStream,
            IEnumerable<EventStreamEvent> eventsToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            eventsToAppend = eventsToAppend
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    validSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();

            eventStream.AppendEvents(eventsToAppend);
            
            
            var expectedValue =
                $"Event Stream ID: {eventStream.StreamId}, Current Sequence: {eventStream.CurrentSequence}, Number of events to append: {eventStream.EventsToAppend.Count}";

            Assert.Equal(expectedValue, eventStream.ToString());
        }
    }
}