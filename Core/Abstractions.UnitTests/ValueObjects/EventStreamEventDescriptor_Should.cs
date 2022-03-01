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
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                null,
                serializationFormat,
                eventTypeIdentifier,
                eventTypeIdentifierFormat));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullSerializationFormat(
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                eventContent,
                null,
                eventTypeIdentifier,
                eventTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventTypeIdentifier(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                null,
                eventTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventTypeIdentifierFormat(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            _ = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier,
                eventTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventContentProvidedDuringCreation_When_GettingEventContent(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier,
                eventTypeIdentifierFormat);
            
            Assert.Equal(eventContent, eventStreamEventDescriptor.EventContent);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventContentProvidedDuringCreation_When_GettingEventContentSerializationFormat(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier,
                eventTypeIdentifierFormat);
            
            Assert.Equal(serializationFormat, eventStreamEventDescriptor.EventContentSerializationFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventTypeIdentifierProvidedDuringCreation_When_GettingEventTypeIdentifier(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier,
                eventTypeIdentifierFormat);
            
            Assert.Equal(eventTypeIdentifier, eventStreamEventDescriptor.EventTypeIdentifier);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventTypeIdentifierProvidedDuringCreation_When_GettingEventTypeIdentifierFormat(
            EventStreamEventContent eventContent,
            SerializationFormat serializationFormat,
            EventStreamEventTypeIdentifier eventTypeIdentifier,
            EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                serializationFormat,
                eventTypeIdentifier,
                eventTypeIdentifierFormat);
            
            Assert.Equal(eventTypeIdentifierFormat, eventStreamEventDescriptor.EventTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
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
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
            var event2 = new EventStreamEventDescriptor(
                differentEventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
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
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                serializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
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
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                differentEventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventTypeIdentifierFormat(
            EventStreamEventDescriptor eventStreamEventDescriptor,
            EventStreamEventTypeIdentifierFormat differentEventTypeIdentifierFormat)
        {
            var event1 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier,
                eventStreamEventDescriptor.EventTypeIdentifierFormat);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
                eventStreamEventDescriptor.EventContentSerializationFormat,
                eventStreamEventDescriptor.EventTypeIdentifier,
                differentEventTypeIdentifierFormat);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            var expectedValue =
                $"Event Type Identifier: {eventStreamEventDescriptor.EventTypeIdentifier}, Event Type Identifier Format: {eventStreamEventDescriptor.EventTypeIdentifierFormat}, Event Content Serialization Format: {eventStreamEventDescriptor.EventContentSerializationFormat}, Event Content: {eventStreamEventDescriptor.EventContent}";
            
            Assert.Equal(expectedValue, eventStreamEventDescriptor.ToString());
        }
    }
}