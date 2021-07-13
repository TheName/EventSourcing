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
        public void Throw_ArgumentNullException_When_CreatingWithNullEventContent(
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                null,
                eventTypeIdentifier));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventTypeIdentifier(
            EventStreamEventContent eventContent)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventDescriptor(
                eventContent,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            _ = new EventStreamEventDescriptor(
                eventContent,
                eventTypeIdentifier);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventContentProvidedDuringCreation_When_GettingEventContent(
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
                eventTypeIdentifier);
            
            Assert.Equal(eventContent, eventStreamEventDescriptor.EventContent);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventTypeIdentifierProvidedDuringCreation_When_GettingEventTypeIdentifier(
            EventStreamEventContent eventContent,
            EventStreamEventTypeIdentifier eventTypeIdentifier)
        {
            var eventStreamEventDescriptor = new EventStreamEventDescriptor(
                eventContent,
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
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
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
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                differentEventContent,
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
                eventStreamEventDescriptor.EventTypeIdentifier);
            
            var event2 = new EventStreamEventDescriptor(
                eventStreamEventDescriptor.EventContent,
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
                $"Event Content: {eventStreamEventDescriptor.EventContent}, Event Type Identifier: {eventStreamEventDescriptor.EventTypeIdentifier}";
            
            Assert.Equal(expectedValue, eventStreamEventDescriptor.ToString());
        }
    }
}