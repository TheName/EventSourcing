using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Cleanup;
using EventSourcing.ForgettablePayloads.Configurations;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.ForgettablePayloads.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Cleanup
{
    public class UnclaimedForgettablePayloadsCleanupJob_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_StorageReaderIsNull(
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadConfiguration configuration,
            ILogger<UnclaimedForgettablePayloadsCleanupJob> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupJob(
                null,
                forgettingService,
                configuration,
                logger));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ForgettingServiceIsNull(
            IForgettablePayloadStorageReader storageReader,
            IForgettablePayloadConfiguration configuration,
            ILogger<UnclaimedForgettablePayloadsCleanupJob> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupJob(
                storageReader,
                null,
                configuration,
                logger));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConfigurationIsNull(
            IForgettablePayloadStorageReader storageReader,
            IForgettablePayloadForgettingService forgettingService,
            ILogger<UnclaimedForgettablePayloadsCleanupJob> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupJob(
                storageReader,
                forgettingService,
                null,
                logger));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_LoggerIsNull(
            IForgettablePayloadStorageReader storageReader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupJob(
                storageReader,
                forgettingService,
                configuration,
                null));
        }

        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IForgettablePayloadStorageReader storageReader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadConfiguration configuration,
            ILogger<UnclaimedForgettablePayloadsCleanupJob> logger)
        {
            _ = new UnclaimedForgettablePayloadsCleanupJob(
                storageReader,
                forgettingService,
                configuration,
                logger);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_And_DoNothing_When_Executing_And_StorageReaderReturnsNull(
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as IReadOnlyCollection<ForgettablePayloadDescriptor>)
                .Verifiable();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                cleanupJob.ExecuteAsync(CancellationToken.None));

            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Executing_And_StorageReaderReturnsEmptyDescriptorsCollection(
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ForgettablePayloadDescriptor>())
                .Verifiable();

            await cleanupJob.ExecuteAsync(CancellationToken.None);

            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_EveryDescriptorWasModifiedEarlierThanCleanupTimeout(
            List<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            var now = DateTime.UtcNow;
            var lastModifiedTime = now - TimeSpan.FromMilliseconds(10);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupTimeout)
                .Returns(TimeSpan.FromSeconds(10))
                .Verifiable();

            descriptors = descriptors
                .Select(descriptor => CreateDescriptorWithLastModifiedTime(descriptor, lastModifiedTime))
                .ToList();

            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();

            await cleanupJob.ExecuteAsync(CancellationToken.None);

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryToForgetEveryDescriptor_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_EveryDescriptorWasModifiedGreaterThanCleanupTimeout(
            List<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            var now = DateTime.UtcNow;
            var lastModifiedTime = now - TimeSpan.FromSeconds(10);
            var timeout = TimeSpan.FromSeconds(1);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupTimeout)
                .Returns(timeout)
                .Verifiable();

            descriptors = descriptors
                .Select(descriptor => CreateDescriptorWithLastModifiedTime(descriptor, lastModifiedTime))
                .ToList();

            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();

            await cleanupJob.ExecuteAsync(CancellationToken.None);

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            foreach (var forgettablePayloadDescriptor in descriptors)
            {
                forgettingServiceMock
                    .Verify(service => service.ForgetAsync(
                            forgettablePayloadDescriptor,
                            ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeout),
                            ForgettingPayloadRequestedBy.UnclaimedForgettablePayloadCleanupJob,
                            It.IsAny<CancellationToken>()),
                        Times.Once);
            }
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task TryToForgetSingleDescriptor_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_OnlyOneDescriptorWasModifiedGreaterThanCleanupTimeout(
            int indexOfDescriptorThatWasModifiedEarlierThanRequiredTimeout,
            List<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            var now = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(5);
            var earlierThanRequiredTimeoutLastModifiedTime = now - TimeSpan.FromSeconds(10);
            var laterThanRequiredTimeoutLastModifiedTime = now - TimeSpan.FromSeconds(1);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupTimeout)
                .Returns(timeout)
                .Verifiable();

            descriptors = descriptors
                .Select((descriptor, i) => CreateDescriptorWithLastModifiedTime(descriptor,
                    i == indexOfDescriptorThatWasModifiedEarlierThanRequiredTimeout
                        ? earlierThanRequiredTimeoutLastModifiedTime
                        : laterThanRequiredTimeoutLastModifiedTime))
                .ToList();

            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();

            await cleanupJob.ExecuteAsync(CancellationToken.None);

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            var expectedDescriptor = descriptors[indexOfDescriptorThatWasModifiedEarlierThanRequiredTimeout];
            forgettingServiceMock
                .Verify(service => service.ForgetAsync(
                        expectedDescriptor,
                        ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeout),
                        ForgettingPayloadRequestedBy.UnclaimedForgettablePayloadCleanupJob,
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryToForgetEveryDescriptor_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_EveryDescriptorWasModifiedGreaterThanCleanupTimeout_And_ForgettingServiceThrowsExceptions(
            List<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            var now = DateTime.UtcNow;
            var lastModifiedTime = now - TimeSpan.FromSeconds(10);
            var timeout = TimeSpan.FromSeconds(1);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupTimeout)
                .Returns(timeout)
                .Verifiable();

            descriptors = descriptors
                .Select(descriptor => CreateDescriptorWithLastModifiedTime(descriptor, lastModifiedTime))
                .ToList();

            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();
            foreach (var forgettablePayloadDescriptor in descriptors)
            {
                forgettingServiceMock
                    .Setup(service => service.ForgetAsync(
                        forgettablePayloadDescriptor,
                        ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeout),
                        ForgettingPayloadRequestedBy.UnclaimedForgettablePayloadCleanupJob,
                        It.IsAny<CancellationToken>()))
                    .Throws<Exception>()
                    .Verifiable();
            }

            await cleanupJob.ExecuteAsync(CancellationToken.None);

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.Verify();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task Throw_OperationCancelledException_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_EveryDescriptorWasModifiedGreaterThanCleanupTimeout_And_ForgettingServiceThrowsOperationCancelledExceptions_And_CancellationWasRequested(
            int indexOfDescriptorThatThrows,
            List<ForgettablePayloadDescriptor> descriptors,
            OperationCanceledException stoppingException,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            var now = DateTime.UtcNow;
            var lastModifiedTime = now - TimeSpan.FromSeconds(10);
            var timeout = TimeSpan.FromSeconds(1);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupTimeout)
                .Returns(timeout)
                .Verifiable();

            descriptors = descriptors
                .Select(descriptor => CreateDescriptorWithLastModifiedTime(descriptor, lastModifiedTime))
                .ToList();

            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            foreach (var descriptor in descriptors)
            {
                var returningFunc =
                    new Func<Task<ForgettablePayloadDescriptor>>(() =>
                    {
                        if (descriptors.IndexOf(descriptor) == indexOfDescriptorThatThrows)
                        {
                            cancellationTokenSource.Cancel();
                            return Task.FromException<ForgettablePayloadDescriptor>(stoppingException);
                        }

                        return Task.FromException<ForgettablePayloadDescriptor>(new OperationCanceledException());
                    });

                forgettingServiceMock
                    .Setup(service => service.ForgetAsync(
                        descriptor,
                        ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeout),
                        ForgettingPayloadRequestedBy.UnclaimedForgettablePayloadCleanupJob,
                        It.IsAny<CancellationToken>()))
                    .Returns(returningFunc)
                    .Verifiable();

                if (indexOfDescriptorThatThrows == descriptors.IndexOf(descriptor))
                {
                    break;
                }
            }

            var thrownException =
                await Assert.ThrowsAsync<OperationCanceledException>(() =>
                    cleanupJob.ExecuteAsync(cancellationTokenSource.Token));

            Assert.Equal(stoppingException, thrownException);
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.Verify();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        internal async Task Throw_OperationCancelledException_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_EveryDescriptorWasModifiedGreaterThanCleanupTimeout_And_ForgettingServiceDoesNotThrow_And_CancellationWasRequested(
            int indexOfDescriptorAfterWhichCancellationWasRequested,
            List<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            var now = DateTime.UtcNow;
            var lastModifiedTime = now - TimeSpan.FromSeconds(10);
            var timeout = TimeSpan.FromSeconds(1);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupTimeout)
                .Returns(timeout)
                .Verifiable();

            descriptors = descriptors
                .Select(descriptor => CreateDescriptorWithLastModifiedTime(descriptor, lastModifiedTime))
                .ToList();

            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            foreach (var descriptor in descriptors)
            {
                var returningFunc =
                    new Func<Task<ForgettablePayloadDescriptor>>(() =>
                    {
                        if (descriptors.IndexOf(descriptor) == indexOfDescriptorAfterWhichCancellationWasRequested)
                        {
                            cancellationTokenSource.Cancel();
                        }

                        return Task.FromResult(null as ForgettablePayloadDescriptor);
                    });

                forgettingServiceMock
                    .Setup(service => service.ForgetAsync(
                        descriptor,
                        ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeout),
                        ForgettingPayloadRequestedBy.UnclaimedForgettablePayloadCleanupJob,
                        It.IsAny<CancellationToken>()))
                    .Returns(returningFunc)
                    .Verifiable();

                if (indexOfDescriptorAfterWhichCancellationWasRequested == descriptors.IndexOf(descriptor))
                {
                    break;
                }
            }

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                cleanupJob.ExecuteAsync(cancellationTokenSource.Token));

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.Verify();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_OperationCancelledException_When_Executing_And_StorageReaderReturnsNonEmptyDescriptorsCollection_And_CancellationWasRequested(
            List<ForgettablePayloadDescriptor> descriptors,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            UnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            storageReaderMock
                .Setup(reader => reader.ReadUnclaimedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptors)
                .Verifiable();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                cleanupJob.ExecuteAsync(new CancellationToken(true)));

            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        private static ForgettablePayloadDescriptor CreateDescriptorWithLastModifiedTime(
            ForgettablePayloadDescriptor descriptor,
            DateTime lastModifiedTime)
        {
            return new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                lastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
        }
    }
}
