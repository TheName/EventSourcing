using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadMetadata_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamId(
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                null,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEntryId(
            EventStreamId eventStreamId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                eventStreamId,
                null,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadId(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                null,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadState(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                null,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadCreationTime(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                null,
                payloadLastModifiedTime,
                payloadSequence));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadLastModifiedTime(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadSequence payloadSequence)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                null,
                payloadSequence));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadSequence(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            _ = new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValuesProvidedDuringCreation_When_GettingPropertiesValues(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            var payloadDescriptor = new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence);
            
            Assert.Equal(eventStreamId, payloadDescriptor.EventStreamId);
            Assert.Equal(eventStreamEntryId, payloadDescriptor.EventStreamEntryId);
            Assert.Equal(payloadId, payloadDescriptor.PayloadId);
            Assert.Equal(payloadState, payloadDescriptor.PayloadState);
            Assert.Equal(payloadCreationTime, payloadDescriptor.PayloadCreationTime);
            Assert.Equal(payloadLastModifiedTime, payloadDescriptor.PayloadLastModifiedTime);
            Assert.Equal(payloadSequence, payloadDescriptor.PayloadSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence);
            
            Assert.Equal(metadata1, metadata2);
            Assert.True(metadata1 == metadata2);
            Assert.False(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventStreamId(
            ForgettablePayloadMetadata metadata,
            EventStreamId differentEventStreamId)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                differentEventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventStreamEntryId(
            ForgettablePayloadMetadata metadata,
            EventStreamEntryId differentEventStreamEntryId)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                differentEventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadId(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadId differentPayloadId)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                differentPayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadState(
            ForgettablePayloadMetadata metadata)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);

            var differentPayloadState = GetDifferentState(metadata.PayloadState);
            
            var metadata2 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                differentPayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadCreationTime(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadCreationTime differentPayloadCreationTime)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                differentPayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadLastModifiedTime(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadLastModifiedTime differentPayloadLastModifiedTime)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                differentPayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadSequence(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadSequence differentPayloadSequence)
        {
            var metadata1 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence);
            
            var metadata2 = new ForgettablePayloadMetadata(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                differentPayloadSequence);
            
            Assert.NotEqual(metadata1, metadata2);
            Assert.False(metadata1 == metadata2);
            Assert.True(metadata1 != metadata2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnNewMetadataObjectWithProvidedStateAndCurrentLastModifiedTimeAndIncreasedSequence(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadState newState)
        {
            var newMetadata = metadata.CreateUpdated(newState);
            
            Assert.Equal(metadata.EventStreamId, newMetadata.EventStreamId);
            Assert.Equal(metadata.EventStreamEntryId, newMetadata.EventStreamEntryId);
            Assert.Equal(metadata.PayloadId, newMetadata.PayloadId);
            Assert.Equal(newState, newMetadata.PayloadState);
            Assert.Equal(metadata.PayloadCreationTime, newMetadata.PayloadCreationTime);
            Assert.True((DateTime.UtcNow - newMetadata.PayloadLastModifiedTime).TotalMilliseconds <= 10);
            Assert.NotEqual(metadata.PayloadLastModifiedTime, newMetadata.PayloadLastModifiedTime);
            Assert.Equal(metadata.PayloadSequence + 1, newMetadata.PayloadSequence.Value);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(ForgettablePayloadMetadata metadata)
        {
            var expectedValue =
                $"Forgettable payload's metadata related to event stream id {metadata.EventStreamId} and entry id {metadata.EventStreamEntryId}, Payload Id {metadata.PayloadId}, Payload State: {metadata.PayloadState}, Payload Creation Time: {metadata.PayloadCreationTime}, Payload Last Modified Time: {metadata.PayloadLastModifiedTime}, Payload Sequence: {metadata.PayloadSequence}";
            
            Assert.Equal(expectedValue, metadata.ToString());
        }

        private static ForgettablePayloadState GetDifferentState(ForgettablePayloadState state)
        {
            if (state == ForgettablePayloadState.Created)
            {
                return new Random().Next(0, 2) == 0
                    ? ForgettablePayloadState.CreatedAndClaimed
                    : ForgettablePayloadState.Forgotten;
            }

            if (state == ForgettablePayloadState.CreatedAndClaimed)
            {
                return new Random().Next(0, 2) == 0
                    ? ForgettablePayloadState.Created
                    : ForgettablePayloadState.Forgotten;
            }

            return new Random().Next(0, 2) == 0
                ? ForgettablePayloadState.Created
                : ForgettablePayloadState.CreatedAndClaimed;
        }
    }
}
