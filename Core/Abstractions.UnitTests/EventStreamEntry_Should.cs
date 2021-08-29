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
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                null,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventId(
            EventStreamId streamId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                null,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventSequence(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                null,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventDescriptor(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                null,
                causationId,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEntryCausationId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                null,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEntryCreationTime(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                null,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEntryCorrelationId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            _ = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(streamId, eventStreamEvent.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventIdProvidedDuringCreation_When_GettingEventId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(eventId, eventStreamEvent.EntryId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventSequenceProvidedDuringCreation_When_GettingEventSequence(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(eventSequence, eventStreamEvent.EntrySequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventProvidedDuringCreation_When_GettingEventDescriptor(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(eventDescriptor, eventStreamEvent.EventDescriptor);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingCausationId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(causationId, eventStreamEvent.CausationId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingCreationTime(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(creationTime, eventStreamEvent.CreationTime);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingCorrelationId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEventDescriptor eventDescriptor,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventStreamEvent = new EventStreamEntry(
                streamId,
                eventId,
                eventSequence,
                eventDescriptor,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(correlationId, eventStreamEvent.CorrelationId);
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
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
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
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                differentStreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
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
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                differentEventId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
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
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                differentEventSequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
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
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                differentEventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCausationId(
            EventStreamEntry eventStreamEvent,
            EventStreamEntryCausationId differentCausationId)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                differentCausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCreationTime(
            EventStreamEntry eventStreamEvent,
            EventStreamEntryCreationTime differentCreationTime)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                differentCreationTime,
                eventStreamEvent.CorrelationId);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCorrelationId(
            EventStreamEntry eventStreamEvent,
            EventStreamEntryCorrelationId differentCorrelationId)
        {
            var event1 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                eventStreamEvent.CorrelationId);
            
            var event2 = new EventStreamEntry(
                eventStreamEvent.StreamId,
                eventStreamEvent.EntryId,
                eventStreamEvent.EntrySequence,
                eventStreamEvent.EventDescriptor,
                eventStreamEvent.CausationId,
                eventStreamEvent.CreationTime,
                differentCorrelationId);
            
            Assert.NotEqual(event1, event2);
            Assert.False(event1 == event2);
            Assert.True(event1 != event2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEntry eventStreamEntry)
        {
            var expectedValue =
                $"Event Stream ID: {eventStreamEntry.StreamId}, Entry ID: {eventStreamEntry.EntryId}, Entry Sequence: {eventStreamEntry.EntrySequence}, Event Descriptor: {eventStreamEntry.EventDescriptor}, Causation ID: {eventStreamEntry.CausationId}, Creation Time: {eventStreamEntry.CreationTime}, Correlation ID: {eventStreamEntry.CorrelationId}";
            
            Assert.Equal(expectedValue, eventStreamEntry.ToString());
        }
    }
}