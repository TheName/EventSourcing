using System;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEntryMetadata_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCausationId(
            EventStreamEntryCreationTime creationTime,
            CorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntryMetadata(
                null,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCreationTime(
            CausationId causationId,
            CorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntryMetadata(
                causationId,
                null,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCorrelationId(
            CausationId causationId,
            EventStreamEntryCreationTime creationTime)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntryMetadata(
                causationId,
                creationTime,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            CausationId causationId,
            EventStreamEntryCreationTime creationTime,
            CorrelationId correlationId)
        {
            var _ = new EventStreamEntryMetadata(
                causationId,
                creationTime,
                correlationId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            CausationId causationId,
            EventStreamEntryCreationTime creationTime,
            CorrelationId correlationId)
        {
            var metadata1 = new EventStreamEntryMetadata(
                causationId,
                creationTime,
                correlationId);
            
            var metadata2 = new EventStreamEntryMetadata(
                causationId,
                creationTime,
                correlationId);
            
            Assert.Equal(metadata1, metadata2);
            Assert.True(metadata1 == metadata2);
            Assert.False(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCausationId(
            CausationId causationId1,
            CausationId causationId2,
            EventStreamEntryCreationTime creationTime,
            CorrelationId correlationId)
        {
            var metadata1 = new EventStreamEntryMetadata(
                causationId1,
                creationTime,
                correlationId);
            
            var metadata2 = new EventStreamEntryMetadata(
                causationId2,
                creationTime,
                correlationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCreationTime(
            CausationId causationId,
            EventStreamEntryCreationTime creationTime1,
            EventStreamEntryCreationTime creationTime2,
            CorrelationId correlationId)
        {
            var metadata1 = new EventStreamEntryMetadata(
                causationId,
                creationTime1,
                correlationId);
            
            var metadata2 = new EventStreamEntryMetadata(
                causationId,
                creationTime2,
                correlationId);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentCorrelationId(
            CausationId causationId,
            EventStreamEntryCreationTime creationTime,
            CorrelationId correlationId1,
            CorrelationId correlationId2)
        {
            var metadata1 = new EventStreamEntryMetadata(
                causationId,
                creationTime,
                correlationId1);
            
            var metadata2 = new EventStreamEntryMetadata(
                causationId,
                creationTime,
                correlationId2);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEntryMetadata eventStreamEntryMetadata)
        {
            var expectedValue =
                $"Causation ID: {eventStreamEntryMetadata.CausationId}, Creation Time: {eventStreamEntryMetadata.CreationTime}, Correlation ID: {eventStreamEntryMetadata.CorrelationId}";
            
            Assert.Equal(expectedValue, eventStreamEntryMetadata.ToString());
        }
    }
}