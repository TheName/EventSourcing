using System;
using EventSourcing.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEventMetadata_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCausationId(
            EventStreamEventCreationTime creationTime,
            EventStreamEventCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                null,
                creationTime,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCausationId_And_WithoutCreationTime(
            EventStreamEventCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                null,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCreationTime(
            EventStreamEventCausationId causationId,
            EventStreamEventCorrelationId correlationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                causationId,
                null,
                correlationId));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCorrelationId(
            EventStreamEventCausationId causationId,
            EventStreamEventCreationTime creationTime)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                causationId,
                creationTime,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullCorrelationId_And_WithoutCreationTime(
            EventStreamEventCausationId causationId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventMetadata(
                causationId,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamEventCausationId causationId,
            EventStreamEventCreationTime creationTime,
            EventStreamEventCorrelationId correlationId)
        {
            var _ = new EventStreamEventMetadata(
                causationId,
                creationTime,
                correlationId);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues_And_WithoutCreationTime(
            EventStreamEventCausationId causationId,
            EventStreamEventCorrelationId correlationId)
        {
            var _ = new EventStreamEventMetadata(
                causationId,
                correlationId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamEventCausationId causationId,
            EventStreamEventCreationTime creationTime,
            EventStreamEventCorrelationId correlationId)
        {
            var metadata1 = new EventStreamEventMetadata(
                causationId,
                creationTime,
                correlationId);
            
            var metadata2 = new EventStreamEventMetadata(
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
            EventStreamEventCausationId causationId1,
            EventStreamEventCausationId causationId2,
            EventStreamEventCreationTime creationTime,
            EventStreamEventCorrelationId correlationId)
        {
            var metadata1 = new EventStreamEventMetadata(
                causationId1,
                creationTime,
                correlationId);
            
            var metadata2 = new EventStreamEventMetadata(
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
            EventStreamEventCausationId causationId,
            EventStreamEventCreationTime creationTime1,
            EventStreamEventCreationTime creationTime2,
            EventStreamEventCorrelationId correlationId)
        {
            var metadata1 = new EventStreamEventMetadata(
                causationId,
                creationTime1,
                correlationId);
            
            var metadata2 = new EventStreamEventMetadata(
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
            EventStreamEventCausationId causationId,
            EventStreamEventCreationTime creationTime,
            EventStreamEventCorrelationId correlationId1,
            EventStreamEventCorrelationId correlationId2)
        {
            var metadata1 = new EventStreamEventMetadata(
                causationId,
                creationTime,
                correlationId1);
            
            var metadata2 = new EventStreamEventMetadata(
                causationId,
                creationTime,
                correlationId2);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEventMetadata eventStreamEntryMetadata)
        {
            var expectedValue =
                $"Causation ID: {eventStreamEntryMetadata.CausationId}, Creation Time: {eventStreamEntryMetadata.CreationTime}, Correlation ID: {eventStreamEntryMetadata.CorrelationId}";
            
            Assert.Equal(expectedValue, eventStreamEntryMetadata.ToString());
        }
    }
}