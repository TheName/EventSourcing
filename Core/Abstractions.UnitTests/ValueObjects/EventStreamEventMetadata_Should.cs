using System;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEventMetadata_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamId(
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                null,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventId(
            EventStreamId streamId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                streamId,
                null,
                eventSequence,
                causationId,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEventSequence(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                streamId,
                eventId,
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
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
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
            EventStreamEntryCausationId causationId,
            EventStreamEntryCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
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
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
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
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            _ = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
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
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventMetadata = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(streamId, eventMetadata.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventIdProvidedDuringCreation_When_GettingEventId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventMetadata = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(eventId, eventMetadata.EntryId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventSequenceProvidedDuringCreation_When_GettingEventSequence(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventMetadata = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(eventSequence, eventMetadata.EntrySequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingCausationId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventMetadata = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(causationId, eventMetadata.CausationId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingCreationTime(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventMetadata = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(creationTime, eventMetadata.CreationTime);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventMetadataProvidedDuringCreation_When_GettingCorrelationId(
            EventStreamId streamId,
            EventStreamEntryId eventId,
            EventStreamEntrySequence eventSequence,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            var eventMetadata = new EventStreamEventMetadata(
                streamId,
                eventId,
                eventSequence,
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(correlationId, eventMetadata.CorrelationId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEventMetadata eventMetadata)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            Assert.Equal(metadata1, metadata2);
            Assert.True(metadata1 == metadata2);
            Assert.False(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamId(
            EventStreamEventMetadata eventMetadata,
            EventStreamId differentStreamId)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                differentStreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventId(
            EventStreamEventMetadata eventMetadata,
            EventStreamEntryId differentEventId)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                differentEventId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventSequence(
            EventStreamEventMetadata eventMetadata,
            EventStreamEntrySequence differentEventSequence)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                differentEventSequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCausationId(
            EventStreamEventMetadata eventMetadata,
            EventStreamEntryCausationId differentCausationId)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                differentCausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCreationTime(
            EventStreamEventMetadata eventMetadata,
            EventStreamEntryCreationTime differentCreationTime)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                differentCreationTime,
                eventMetadata.CorrelationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCorrelationId(
            EventStreamEventMetadata eventMetadata,
            EventStreamEntryCorrelationId differentCorrelationId)
        {
            var metadata1 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            var metadata2 = new EventStreamEventMetadata(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                differentCorrelationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEventMetadata eventMetadata)
        {
            var expectedValue =
                $"Event Stream ID: {eventMetadata.StreamId}, Entry ID: {eventMetadata.EntryId}, Entry Sequence: {eventMetadata.EntrySequence}, Causation ID: {eventMetadata.CausationId}, Creation Time: {eventMetadata.CreationTime}, Correlation ID: {eventMetadata.CorrelationId}";
            
            Assert.Equal(expectedValue, eventMetadata.ToString());
        }
    }
}