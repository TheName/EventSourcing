using System;
using EventSourcing.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEvent_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamId(
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEvent(
                null,
                eventId,
                eventSequence,
                @event,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventId(
            EventStreamId streamId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEvent(
                streamId,
                null,
                eventSequence,
                @event,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventSequence(
            EventStreamId streamId,
            EventStreamEventId eventId,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEvent(
                streamId,
                eventId,
                null,
                @event,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEvent(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                null,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventMetadata(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            _ = new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                eventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                eventMetadata);
            
            Assert.Equal(streamId, eventStreamEvent.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventIdProvidedDuringCreation_When_GettingEventId(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                eventMetadata);
            
            Assert.Equal(eventId, eventStreamEvent.EventId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventSequenceProvidedDuringCreation_When_GettingEventSequence(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                eventMetadata);
            
            Assert.Equal(eventSequence, eventStreamEvent.EventSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventProvidedDuringCreation_When_GettingEvent(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                eventMetadata);
            
            Assert.Equal(@event, eventStreamEvent.Event);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingEventMetadata(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEvent(
                streamId,
                eventId,
                eventSequence,
                @event,
                eventMetadata);
            
            Assert.Equal(eventMetadata, eventStreamEvent.EventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEvent eventStreamEvent)
        {
            var event1 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            var event2 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            Assert.Equal(event1, event2);
            Assert.True(event1 == event2);
            Assert.False(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamId(
            EventStreamEvent eventStreamEvent,
            EventStreamId differentStreamId)
        {
            var event1 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            var event2 = new EventStreamEvent(
                differentStreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventId(
            EventStreamEvent eventStreamEvent,
            EventStreamEventId differentEventId)
        {
            var event1 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            var event2 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                differentEventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventSequence(
            EventStreamEvent eventStreamEvent,
            EventStreamEventSequence differentEventSequence)
        {
            var event1 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            var event2 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                differentEventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEvent(
            EventStreamEvent eventStreamEvent,
            object differentEvent)
        {
            var event1 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            var event2 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                differentEvent,
                eventStreamEvent.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventMetadata(
            EventStreamEvent eventStreamEvent,
            EventStreamEventMetadata differentEventMetadata)
        {
            var event1 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                eventStreamEvent.EventMetadata);
            
            var event2 = new EventStreamEvent(
                eventStreamEvent.StreamId,
                eventStreamEvent.EventId,
                eventStreamEvent.EventSequence,
                eventStreamEvent.Event,
                differentEventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEvent eventStreamEvent)
        {
            var expectedValue =
                $"Event Stream ID: {eventStreamEvent.StreamId}, Event ID: {eventStreamEvent.EventId}, Event Sequence: {eventStreamEvent.EventSequence}, Event: {eventStreamEvent.Event}, EventMetadata: {eventStreamEvent.EventMetadata}";
            
            Assert.Equal(expectedValue, eventStreamEvent.ToString());
        }
    }
}