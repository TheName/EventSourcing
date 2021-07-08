using System;
using EventSourcing.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEventDescriptor_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamId(
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                null,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventId(
            EventStreamId streamId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                streamId,
                null,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventSequence(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                streamId,
                eventId,
                null,
                eventContent,
                eventTypeIdentifier,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventContent(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                null,
                eventTypeIdentifier,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventTypeIdentifier(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                null,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventMetadata(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            _ = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
            
            Assert.Equal(streamId, eventStreamEventDescriptor.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventIdProvidedDuringCreation_When_GettingEventId(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
            
            Assert.Equal(eventId, eventStreamEventDescriptor.EventId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventSequenceProvidedDuringCreation_When_GettingEventSequence(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
            
            Assert.Equal(eventSequence, eventStreamEventDescriptor.EventSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventProvidedDuringCreation_When_GettingEventContent(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
            
            Assert.Equal(eventContent, eventStreamEventDescriptor.EventContent);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventProvidedDuringCreation_When_GettingEventTypeIdentifier(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
            
            Assert.Equal(eventTypeIdentifier, eventStreamEventDescriptor.EventTypeIdentifier);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingEventMetadata(
            EventStreamId streamId,
            EventStreamEventId eventId,
            EventStreamEventSequence eventSequence,
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventMetadata eventMetadata)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                streamId,
                eventId,
                eventSequence,
                eventContent,
                eventTypeIdentifier,
                eventMetadata);
            
            Assert.Equal(eventMetadata, eventStreamEventDescriptor.EventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            Assert.Equal(event1, event2);
            Assert.True(event1 == event2);
            Assert.False(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamId(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamId differentStreamId)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                differentStreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventId(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventId differentEventId)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                differentEventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventSequence(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventSequence differentEventSequence)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                differentEventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventContent(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventContent differentEventContent)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                differentEventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventTypeIdentifier(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventTypeIdentifier differentEventTypeIdentifier)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                differentEventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventMetadata(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventMetadata differentEventMetadata)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventMetadata);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.StreamId,
                eventStreamEventDescriptor.EventId,
                eventStreamEventDescriptor.EventSequence,
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventTypeIdentifier,
                differentEventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            var expectedValue =
                $"Event Stream ID: {eventStreamEventDescriptor.StreamId}, Event ID: {eventStreamEventDescriptor.EventId}, Event Sequence: {eventStreamEventDescriptor.EventSequence}, Event Content: {eventStreamEventDescriptor.EventContent}, Event Type Identifier: {eventStreamEventDescriptor.EventTypeIdentifier}, EventMetadata: {eventStreamEventDescriptor.EventMetadata}";
            
            Assert.Equal(expectedValue, eventStreamEventDescriptor.ToString());
        }
    }
}