using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Persistence;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Persistence.UnitTests
{
    public class ForgettablePayloadStorageReader_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_TryingToCreateWithNullRepository()
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadStorageReader(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_TryingToCreateWithoutNullParameters(IForgettablePayloadStorageRepository repository)
        {
            _ = new ForgettablePayloadStorageReader(repository);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToReadByNullEventStreamId(
            ForgettablePayloadStorageReader reader)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.ReadAsync(null as EventStreamId, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnResultFromRepository_When_TryingToReadByEventStreamId(
            EventStreamId eventStreamId,
            IReadOnlyCollection<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageReader reader)
        {
            repositoryMock
                .Setup(repository => repository.ReadAsync(eventStreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors);

            var result = await reader.ReadAsync(eventStreamId, CancellationToken.None);

            Assert.Equal(descriptors, result);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToReadByNullEventStreamIdAndNotNullEventStreamEntryId(
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadStorageReader reader)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.ReadAsync(null, eventStreamEntryId, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToReadByNotNullEventStreamIdAndNullEventStreamEntryId(
            EventStreamId eventStreamId,
            ForgettablePayloadStorageReader reader)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.ReadAsync(eventStreamId, null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnResultFromRepository_When_TryingToReadByEventStreamIdAndEventStreamEntryId(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            IReadOnlyCollection<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageReader reader)
        {
            repositoryMock
                .Setup(repository => repository.ReadAsync(eventStreamId, eventStreamEntryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors);

            var result = await reader.ReadAsync(eventStreamId, eventStreamEntryId, CancellationToken.None);

            Assert.Equal(descriptors, result);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToReadByNullForgettablePayloadId(
            ForgettablePayloadStorageReader reader)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.ReadAsync(null as ForgettablePayloadId, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnResultFromRepository_When_TryingToReadByForgettablePayloadId(
            ForgettablePayloadId forgettablePayloadId,
            ForgettablePayloadDescriptor descriptor,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageReader reader)
        {
            repositoryMock
                .Setup(repository => repository.ReadAsync(forgettablePayloadId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptor);

            var result = await reader.ReadAsync(forgettablePayloadId, CancellationToken.None);

            Assert.Equal(descriptor, result);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnResultFromRepository_When_TryingToReadUnclaimed(
            IReadOnlyCollection<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageRepository> repositoryMock,
            ForgettablePayloadStorageReader reader)
        {
            repositoryMock
                .Setup(repository => repository.ReadAsync(ForgettablePayloadState.Created, It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors);

            var result = await reader.ReadUnclaimedAsync(CancellationToken.None);

            Assert.Equal(descriptors, result);
        }
    }
}