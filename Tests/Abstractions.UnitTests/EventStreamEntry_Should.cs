using System;
using EventSourcing.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEntry_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamId(
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                null,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventId(
            EventStreamId streamId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                null,
                eventSequence,
                eventDescriptor,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventSequence(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                null,
                eventDescriptor,
                eventMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventDescriptor(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryMetadata eventMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
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
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            _ = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata);
            
            Assert.Equal(streamId, eventStreamEvent.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventIdProvidedDuringCreation_When_GettingEventId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata);
            
            Assert.Equal(eventId, eventStreamEvent.EntryId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventSequenceProvidedDuringCreation_When_GettingEventSequence(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata);
            
            Assert.Equal(eventSequence, eventStreamEvent.EntrySequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventProvidedDuringCreation_When_GettingEventDescriptor(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata);
            
            Assert.Equal(eventDescriptor, eventStreamEvent.EventDescriptor);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingEventMetadata(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryMetadata eventMetadata)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                eventMetadata);
            
            Assert.Equal(eventMetadata, eventStreamEvent.EntryMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEntry eventStreamEvent)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            Assert.Equal(event1, event2);
            Assert.True(event1 == event2);
            Assert.False(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamId(
            EventStreamEntry eventStreamEvent,
            EventStreamId differentStreamId)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            var event2 = new EventStreamEntry(
                differentStreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventId(
            EventStreamEntry eventStreamEvent,
            EventStreamEntryId differentEventId)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                differentEventId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventSequence(
            EventStreamEntry eventStreamEvent,
            EventStreamEntrySequence differentEventSequence)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                differentEventSequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventDescriptor(
            EventStreamEntry eventStreamEvent,
            EventStreamEventDescriptor differentEventDescriptor)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                differentEventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventMetadata(
            EventStreamEntry eventStreamEvent,
            EventStreamEntryMetadata differentEventMetadata)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.EntryMetadata);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                differentEventMetadata);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEntry eventStreamEntry)
        {
            var expectedValue =
                $"Event Stream ID: {eventStreamEntry.StreamId}, Entry ID: {eventStreamEntry.EntryId}, Entry Sequence: {eventStreamEntry.EntrySequence}, Event Descriptor: {eventStreamEntry.EventDescriptor}, Entry Metadata: {eventStreamEntry.EntryMetadata}";
            
            Assert.Equal(expectedValue, eventStreamEntry.ToString());
        }
    }
}