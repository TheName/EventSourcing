using System;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEventDescriptor_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventContent(
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                null,
                serializationFormat,
                eventTypeIdentifier));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullSerializationFormat(
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                eventContent,
                null,
                eventTypeIdentifier));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventTypeIdentifier(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            _ = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventContentProvidedDuringCreation_When_GettingEventContent(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier);
            
            Assert.Equal(eventContent, eventStreamEventDescriptor.EventContent);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventTypeIdentifierProvidedDuringCreation_When_GettingEventTypeIdentifier(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier);
            
            Assert.Equal(eventTypeIdentifier, eventStreamEventDescriptor.EventTypeIdentifier);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            Assert.Equal(event1, event2);
            Assert.True(event1 == event2);
            Assert.False(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventContent(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventContent differentEventContent)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                differentEventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventContentSerializationFormat(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            SerializationFormat serializationFormat)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                serializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
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
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                differentEventTypeIdentifier);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            var expectedValue =
                $"Event Content Serialization Format: {eventStreamEventDescriptor.EventContentSerializationFormat}, Event Content: {eventStreamEventDescriptor.EventContent}, Event Type Identifier: {eventStreamEventDescriptor.EventTypeIdentifier}";
            
            Assert.Equal(expectedValue, eventStreamEventDescriptor.ToString());
        }
    }
}