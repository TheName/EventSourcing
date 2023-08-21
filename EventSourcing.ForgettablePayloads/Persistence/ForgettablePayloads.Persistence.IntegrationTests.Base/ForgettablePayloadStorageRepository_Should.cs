using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Persistence.IntegrationTests.Base
{
    public abstract class ForgettablePayloadStorageRepository_Should
    {
        protected abstract IForgettablePayloadStorageRepository Repository { get; }

        [Theory]
        [AutoMoqData]
        public async Task ReturnEmptyCollection_When_TryingToReadByEventStreamIdThatDoesNotExist(EventStreamId eventStreamId)
        {
            var result = await Repository.ReadAsync(eventStreamId, CancellationToken.None);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnTrue_When_InsertingDescriptor(ForgettablePayloadDescriptor descriptor)
        {
            var result = await Repository.TryInsertAsync(descriptor, CancellationToken.None);

            Assert.True(result);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptor_When_InsertingNewDescriptor_And_ReadingByEventStreamId(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var readingResult = await Repository.ReadAsync(descriptor.EventStreamId, CancellationToken.None);
            var singleReadingResult = Assert.Single(readingResult);

            Assert.Equal(descriptor, singleReadingResult);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptors_When_InsertingMultipleNewDescriptorWithSameEventStreamId_And_ReadingByEventStreamId(
            [Frozen] EventStreamId eventStreamId,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(eventStreamId, CancellationToken.None);
            Assert.Equal(descriptors.Count, readingResult.Count);
            Assert.All(descriptors, descriptor => readingResult.Contains(descriptor));
        }
        
        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public async Task ReturnInsertedDescriptor_When_InsertingMultipleNewDescriptorWithDifferentEventStreamIds_And_ReadingByEventStreamId(
            int indexOfDescriptorToRead,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(descriptors[indexOfDescriptorToRead].EventStreamId, CancellationToken.None);
            var singleReadingResult = Assert.Single(readingResult);
            
            Assert.Equal(descriptors[indexOfDescriptorToRead], singleReadingResult);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToInsertSameDescriptorMoreThanOnce(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            await Assert.ThrowsAnyAsync<Exception>(() => Repository.TryInsertAsync(descriptor, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToInsertDifferentDescriptorsWithSamePayloadId(
            [Frozen] ForgettablePayloadId forgettablePayloadId,
            ForgettablePayloadDescriptor firstDescriptor,
            ForgettablePayloadDescriptor secondDescriptor)
        {
            Assert.Equal(forgettablePayloadId, firstDescriptor.PayloadId);
            Assert.Equal(forgettablePayloadId, secondDescriptor.PayloadId);
            
            var insertionResult = await Repository.TryInsertAsync(firstDescriptor, CancellationToken.None);
            Assert.True(insertionResult);

            await Assert.ThrowsAnyAsync<Exception>(() => Repository.TryInsertAsync(secondDescriptor, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnEmptyCollection_When_TryingToReadByEventStreamIdAndEventStreamEntryIdThatDoNotExist(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId)
        {
            var result = await Repository.ReadAsync(
                eventStreamId,
                eventStreamEntryId,
                CancellationToken.None);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnEmptyCollection_When_TryingToReadByEventStreamIdThatExistsAndEventStreamEntryIdThatDoesNotExist(
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var result = await Repository.ReadAsync(
                descriptor.EventStreamId,
                eventStreamEntryId,
                CancellationToken.None);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnEmptyCollection_When_TryingToReadByEventStreamIdThatDoesNotExistAndEventStreamEntryIdThatExists(
            EventStreamId eventStreamId,
            ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var result = await Repository.ReadAsync(
                eventStreamId,
                descriptor.EventStreamEntryId,
                CancellationToken.None);

            Assert.Empty(result);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptor_When_InsertingNewDescriptor_And_ReadingByEventStreamIdAndEventStreamEntryId(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var readingResult = await Repository.ReadAsync(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                CancellationToken.None);
            
            var singleReadingResult = Assert.Single(readingResult);

            Assert.Equal(descriptor, singleReadingResult);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptors_When_InsertingMultipleNewDescriptorWithSameEventStreamIdAndEventStreamEntryId_And_ReadingByEventStreamIdAndEventStreamEntryId(
            [Frozen] EventStreamId eventStreamId,
            [Frozen] EventStreamEntryId eventStreamEntryId,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(eventStreamId, eventStreamEntryId, CancellationToken.None);
            Assert.Equal(descriptors.Count, readingResult.Count);
            Assert.All(descriptors, descriptor => readingResult.Contains(descriptor));
        }
        
        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public async Task ReturnInsertedDescriptor_When_InsertingMultipleNewDescriptorWithSameEventStreamIdsAndDifferentEventStreamEntryIds_And_ReadingByEventStreamIdAndEventStreamEntryId(
            int indexOfDescriptorToRead,
            [Frozen] EventStreamId eventStreamId,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(
                eventStreamId,
                descriptors[indexOfDescriptorToRead].EventStreamEntryId,
                CancellationToken.None);
            
            var singleReadingResult = Assert.Single(readingResult);
            
            Assert.Equal(descriptors[indexOfDescriptorToRead], singleReadingResult);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnNull_When_TryingToReadByForgettablePayloadIdThatDoNotExist(
            ForgettablePayloadId forgettablePayloadId)
        {
            var result = await Repository.ReadAsync(forgettablePayloadId, CancellationToken.None);

            Assert.Null(result);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptor_When_InsertingNewDescriptor_And_ReadingByForgettablePayloadId(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var readingResult = await Repository.ReadAsync(descriptor.PayloadId, CancellationToken.None);

            Assert.Equal(descriptor, readingResult);
        }
        
        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public async Task ReturnInsertedDescriptor_When_InsertingMultipleNewDescriptors_And_ReadingByForgettablePayloadId(
            int indexOfDescriptorToRead,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(
                descriptors[indexOfDescriptorToRead].PayloadId,
                CancellationToken.None);
            
            Assert.Equal(descriptors[indexOfDescriptorToRead], readingResult);
        }

        [Theory]
        [InlineData(nameof(ForgettablePayloadState.Created))]
        [InlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [InlineData(nameof(ForgettablePayloadState.Forgotten))]
        public async Task ReturnEmptyCollection_When_TryingToReadByStateThatDoesNotExistInRepository(string state)
        {
            await TruncateAsync(CancellationToken.None);
            
            var result = await Repository.ReadAsync(state, CancellationToken.None);

            Assert.Empty(result);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptor_When_InsertingNewDescriptor_And_ReadingByState(ForgettablePayloadDescriptor descriptor)
        {
            await TruncateAsync(CancellationToken.None);
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var readingResult = await Repository.ReadAsync(descriptor.PayloadState, CancellationToken.None);
            var singleReadingResult = Assert.Single(readingResult);

            Assert.Equal(descriptor, singleReadingResult);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task ReturnInsertedDescriptors_When_InsertingMultipleNewDescriptorsWithSameState_And_ReadingByState(
            [Frozen] ForgettablePayloadState forgettablePayloadState,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await TruncateAsync(CancellationToken.None);
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(forgettablePayloadState, CancellationToken.None);
            Assert.Equal(descriptors.Count, readingResult.Count);
            Assert.All(descriptors, descriptor => readingResult.Contains(descriptor));
        }
        
        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public async Task ReturnInsertedDescriptor_When_InsertingMultipleNewDescriptorsWithDifferentStates_And_ReadingByState(
            int indexToSelect,
            List<ForgettablePayloadDescriptor> descriptors)
        {
            await TruncateAsync(CancellationToken.None);
            var allowedStates = new List<ForgettablePayloadState>
            {
                ForgettablePayloadState.Created,
                ForgettablePayloadState.Forgotten,
                ForgettablePayloadState.CreatedAndClaimed
            };

            descriptors = descriptors
                .Select((descriptor, i) => ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                    descriptor.ToMetadata().CreateUpdated(allowedStates[i]),
                    descriptor.ToContentDescriptor()))
                .ToList();
            
            await Task.WhenAll(descriptors
                .Select(async descriptor =>
                {
                    var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
                    Assert.True(insertionResult);
                }));

            var readingResult = await Repository.ReadAsync(descriptors[indexToSelect].PayloadState, CancellationToken.None);
            var singleReadingResult = Assert.Single(readingResult);

            Assert.Equal(descriptors[indexToSelect], singleReadingResult);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_TryingToUpdateNotInsertedDescriptor(ForgettablePayloadDescriptor descriptor)
        {
            var result = await Repository.TryUpdateAsync(descriptor, CancellationToken.None);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_TryingToUpdateInsertedDescriptorWithoutUpdatingItsSequence(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);
            
            var result = await Repository.TryUpdateAsync(descriptor, CancellationToken.None);
            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnTrue_When_TryingToUpdateInsertedDescriptorWithSequenceIncreasedByOne(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var descriptorWithUpdatedSequence = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence.Value + 1,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var result = await Repository.TryUpdateAsync(descriptorWithUpdatedSequence, CancellationToken.None);
            Assert.True(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_TryingToUpdateInsertedDescriptorWithSequenceIncreasedByMoreThanOne(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var descriptorWithUpdatedSequence = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence.Value + (uint)new Random().Next(2, 100),
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var result = await Repository.TryUpdateAsync(descriptorWithUpdatedSequence, CancellationToken.None);
            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_TryingToUpdateInsertedDescriptorWithSequenceDecreased(ForgettablePayloadDescriptor descriptor)
        {
            var insertionResult = await Repository.TryInsertAsync(descriptor, CancellationToken.None);
            Assert.True(insertionResult);

            var descriptorWithUpdatedSequence = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence.Value - (uint)new Random().Next(1, (int)descriptor.PayloadSequence.Value),
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var result = await Repository.TryUpdateAsync(descriptorWithUpdatedSequence, CancellationToken.None);
            Assert.False(result);
        }

        protected abstract Task TruncateAsync(CancellationToken cancellationToken);
    }
}