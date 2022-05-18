using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Persistence;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Persistence.UnitTests
{
    public class ForgettablePayloadStorageWriter_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_TryingToCreateWithNullLogger(
            IForgettablePayloadStorageRepository repository)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadStorageWriter(
                repository,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_TryingToCreateWithNullRepository(
            ILogger<ForgettablePayloadStorageWriter> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadStorageWriter(
                null,
                logger));
        }

        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_CreatingWithoutNullParameters(
            IForgettablePayloadStorageRepository repository,
            ILogger<ForgettablePayloadStorageWriter> logger)
        {
            _ = new ForgettablePayloadStorageWriter(repository, logger);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToInsertNullDescriptor(
            ForgettablePayloadStorageWriter writer)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => writer.InsertAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_TryingToInsertDescriptorWithSequenceDifferentThanZero(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadStorageWriter writer,
            Random random)
        {
            descriptor = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                (uint)random.Next(1, int.MaxValue),
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => writer.InsertAsync(descriptor, CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        internal async Task Throw_InvalidOperationException_When_TryingToInsertDescriptorWithSequenceEqualToZeroAndStateDifferentThanCreated(
            string state,
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadStorageWriter writer)
        {
            descriptor = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                state,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                0,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => writer.InsertAsync(descriptor, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothingAfterTryingToInsertToRepository_When_TryingToInsertDescriptorWithSequenceEqualToZeroAndStateSetToCreated_And_RepositoryReturnsTrue(
            ForgettablePayloadDescriptor descriptor,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageWriter writer)
        {
            descriptor = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                ForgettablePayloadState.Created, 
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                0,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            repositoryMock
                .Setup(repository => repository.TryInsertAsync(descriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            await writer.InsertAsync(descriptor, CancellationToken.None);
            
            repositoryMock.Verify();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowExceptionAfterTryingToInsertToRepository_When_TryingToInsertDescriptorWithSequenceEqualToZeroAndStateSetToCreated_And_RepositoryReturnsFalse(
            ForgettablePayloadDescriptor descriptor,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageWriter writer)
        {
            descriptor = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                ForgettablePayloadState.Created, 
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                0,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            repositoryMock
                .Setup(repository => repository.TryInsertAsync(descriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .Verifiable();

            await Assert.ThrowsAsync<Exception>(() => writer.InsertAsync(descriptor, CancellationToken.None));
            
            repositoryMock.Verify();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToUpdateNullDescriptor(
            ForgettablePayloadStorageWriter writer)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => writer.UpdateAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothingAfterTryingToUpdateToRepository_When_TryingToUpdate_And_RepositoryReturnsTrue(
            ForgettablePayloadDescriptor descriptor,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageWriter writer)
        {
            repositoryMock
                .Setup(repository => repository.TryUpdateAsync(descriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            await writer.UpdateAsync(descriptor, CancellationToken.None);
            
            repositoryMock.Verify();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowExceptionAfterTryingToUpdateToRepository_When_TryingToUpdate_And_RepositoryReturnsFalse(
            ForgettablePayloadDescriptor descriptor,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageWriter writer)
        {
            repositoryMock
                .Setup(repository => repository.TryUpdateAsync(descriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .Verifiable();

            await Assert.ThrowsAsync<Exception>(() => writer.UpdateAsync(descriptor, CancellationToken.None));
            
            repositoryMock.Verify();
            repositoryMock.VerifyNoOtherCalls();
        }
    }
}