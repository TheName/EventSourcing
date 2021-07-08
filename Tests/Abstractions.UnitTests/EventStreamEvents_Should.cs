using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEvents_Should
    {
        [Fact]
        public void ReturnEmptyEventStreamEvents_When_CallingEmpty()
        {
            var events = EventStreamEvents.Empty;
            
            Assert.Equal<uint>(0, events.MinimumSequence);
            Assert.Equal<uint>(0, events.MaximumSequence);
            Assert.Empty(events);
        }
        
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEvents(null));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_Creating_And_AtLeastOneOfEventsHasInvalidStreamId_And_AllEventsHaveCorrectSequence(
            int invalidStreamIdIndex,
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            IEnumerable<EventStreamEvent> events)
        {
            events = events
                .Select((@event, i) => new EventStreamEvent(
                    i == invalidStreamIdIndex ? @event.StreamId : validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => new EventStreamEvents(events));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamEventSequenceException_When_Creating_And_AllEventsHaveValidStreamId_And_AtLeastOneOfEventsHasInvalidSequence(
            int invalidSequenceIndex,
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            IEnumerable<EventStreamEvent> events)
        {
            var nextSequence = startingSequence;
            var getNextSequenceFunc = new Func<int, EventStreamEvent, EventStreamEventSequence>(
                (currentIndex, @event) =>
                {
                    var currentValidSequence = nextSequence++;
                    if (currentIndex != invalidSequenceIndex)
                    {
                        return currentValidSequence;
                    }

                    var random = new Random();
                    var invalidSequence = random.Next(0, int.MaxValue);
                    while (invalidSequence == currentValidSequence)
                    {
                        invalidSequence = random.Next(0, int.MaxValue);
                    }

                    return Convert.ToUInt32(invalidSequence);
                });
            
            events = events
                .Select((@event, i) => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    getNextSequenceFunc(i, @event),
                    @event.Event,
                    @event.EventMetadata));

            Assert.Throws<InvalidEventStreamEventSequenceException>(() => new EventStreamEvents(events));
        }

        [Fact]
        public void NotThrow_When_CreatingWithEmptyCollection()
        {
            _ = new EventStreamEvents(Array.Empty<EventStreamEvent>());
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithSingleEventStreamEvent(EventStreamEvent eventStreamEvent)
        {
            _ = new EventStreamEvents(new[] {eventStreamEvent});
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithEventsWithSameStreamIdAndCorrectlyIncreasingSequences(
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            IEnumerable<EventStreamEvent> events)
        {
            events = events
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata));
            
            _ = new EventStreamEvents(events);
        }

        [Theory]
        [AutoMoqData]
        public void ContainSingleEvent_When_CreatedWithSingleEvent(EventStreamEvent eventStreamEvent)
        {
            var events = new EventStreamEvents(new[] {eventStreamEvent});

            var singleEvent = Assert.Single(events);
            Assert.NotNull(singleEvent);
            Assert.Equal(eventStreamEvent, singleEvent);
        }

        [Theory]
        [AutoMoqData]
        public void ContainAllEventsInSameSequence_When_CreatedWithMultipleEvents(
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            List<EventStreamEvent> eventsCollection)
        {
            eventsCollection = eventsCollection
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var events = new EventStreamEvents(eventsCollection);

            Assert.Equal(eventsCollection.Count, events.Count);
            Assert.True(eventsCollection.SequenceEqual(events));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsSequence_When_CallingMinimumSequence_And_CreatedWithSingleEvent(EventStreamEvent eventStreamEvent)
        {
            var events = new EventStreamEvents(new[] {eventStreamEvent});
            
            Assert.Equal(eventStreamEvent.EventSequence, events.MinimumSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFirstEventsSequence_When_CallingMinimumSequence_And_CreatedWithMultipleEvents(
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            List<EventStreamEvent> eventsCollection)
        {
            eventsCollection = eventsCollection
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var events = new EventStreamEvents(eventsCollection);
            
            Assert.Equal(eventsCollection.First().EventSequence, events.MinimumSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsSequence_When_CallingMaximumSequence_And_CreatedWithSingleEvent(EventStreamEvent eventStreamEvent)
        {
            var events = new EventStreamEvents(new[] {eventStreamEvent});
            
            Assert.Equal(eventStreamEvent.EventSequence, events.MaximumSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastEventsSequence_When_CallingMaximumSequence_And_CreatedWithMultipleEvents(
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            List<EventStreamEvent> eventsCollection)
        {
            eventsCollection = eventsCollection
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var events = new EventStreamEvents(eventsCollection);
            
            Assert.Equal(eventsCollection.Last().EventSequence, events.MaximumSequence);
        }

        [Fact]
        public void ReturnTrue_When_ComparingDifferentObjectsEmptyCollectionOfEvents()
        {
            var events1 = new EventStreamEvents(new List<EventStreamEvent>());
            var events2 = EventStreamEvents.Empty;
            
            Assert.Equal(events1, events2);
            Assert.True(events1 == events2);
            Assert.False(events1 != events2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameEvents(
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            List<EventStreamEvent> eventsCollection)
        {
            eventsCollection = eventsCollection
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var events1 = new EventStreamEvents(eventsCollection.Select(@event => @event));
            var events2 = new EventStreamEvents(eventsCollection.Select(@event => @event));
            
            Assert.Equal(events1, events2);
            Assert.True(events1 == events2);
            Assert.False(events1 != events2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEvents(
            EventStreamId validStreamId,
            EventStreamEventSequence startingSequence,
            List<EventStreamEvent> eventsCollection1,
            List<EventStreamEvent> eventsCollection2)
        {
            eventsCollection1 = eventsCollection1
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            eventsCollection2 = eventsCollection2
                .Select(@event => new EventStreamEvent(
                    validStreamId,
                    @event.EventId,
                    startingSequence++,
                    @event.Event,
                    @event.EventMetadata))
                .ToList();
            
            var events1 = new EventStreamEvents(eventsCollection1.Select(@event => @event));
            var events2 = new EventStreamEvents(eventsCollection2.Select(@event => @event));
            
            Assert.NotEqual(events1, events2);
            Assert.True(events1 != events2);
            Assert.False(events1 == events2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEvents events)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Events: ");
            foreach (var eventStreamEntry in events)
            {
                stringBuilder.Append($"\n\t{eventStreamEntry}");
            }

            var expectedValue = stringBuilder.ToString();
            
            Assert.Equal(expectedValue, events.ToString());
        }
    }
}