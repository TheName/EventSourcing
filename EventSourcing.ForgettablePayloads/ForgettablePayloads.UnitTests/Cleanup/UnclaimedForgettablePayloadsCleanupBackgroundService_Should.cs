using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Cleanup;
using EventSourcing.ForgettablePayloads.Configurations;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Cleanup
{
    public class UnclaimedForgettablePayloadsCleanupBackgroundService_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConfigurationIsNull(
            IUnclaimedForgettablePayloadsCleanupJob cleanupJob,
            ILogger<UnclaimedForgettablePayloadsCleanupBackgroundService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupBackgroundService(
                null,
                cleanupJob,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_CleanupJobIsNull(
            IForgettablePayloadConfiguration configuration,
            ILogger<UnclaimedForgettablePayloadsCleanupBackgroundService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupBackgroundService(
                configuration,
                null,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_LoggerIsNull(
            IForgettablePayloadConfiguration configuration,
            IUnclaimedForgettablePayloadsCleanupJob cleanupJob)
        {
            Assert.Throws<ArgumentNullException>(() => new UnclaimedForgettablePayloadsCleanupBackgroundService(
                configuration,
                cleanupJob,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IForgettablePayloadConfiguration configuration,
            IUnclaimedForgettablePayloadsCleanupJob cleanupJob,
            ILogger<UnclaimedForgettablePayloadsCleanupBackgroundService> logger)
        {
            _ = new UnclaimedForgettablePayloadsCleanupBackgroundService(
                configuration,
                cleanupJob,
                logger);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Starting_And_CancellationIsAlreadyRequested(
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            [Frozen] Mock<IUnclaimedForgettablePayloadsCleanupJob> cleanupJobMock,
            UnclaimedForgettablePayloadsCleanupBackgroundService reconciliationService)
        {
            var cancellationToken = new CancellationToken(true);

            await reconciliationService.StartAsync(cancellationToken);
            
            configurationMock.VerifyNoOtherCalls();
            cleanupJobMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task ExecuteCleanupJob_When_Started(
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            [Frozen] Mock<IUnclaimedForgettablePayloadsCleanupJob> cleanupJobMock,
            UnclaimedForgettablePayloadsCleanupBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(250);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();
            
            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(jobExecutionInterval, cancellationToken);
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            cleanupJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteCleanupJobInALoop_When_Started(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            [Frozen] Mock<IUnclaimedForgettablePayloadsCleanupJob> cleanupJobMock,
            UnclaimedForgettablePayloadsCleanupBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(150);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();
            
            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * expectedNumberOfReconciliationJobInvocations + jobExecutionInterval / 2,
                cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            cleanupJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedNumberOfReconciliationJobInvocations));
        }
        
        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteCleanupJobInALoopUntilStopped_When_Started(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            [Frozen] Mock<IUnclaimedForgettablePayloadsCleanupJob> cleanupJobMock,
            UnclaimedForgettablePayloadsCleanupBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(50);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();
            
            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * (expectedNumberOfReconciliationJobInvocations + 1) + jobExecutionInterval / 2,
                cancellationToken);

            await reconciliationService.StopAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * expectedNumberOfReconciliationJobInvocations,
                cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            cleanupJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedNumberOfReconciliationJobInvocations));
        }
        
        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteCleanupJobInALoopUntilStoppedEventIfExecutionThrowsOperationCancelledException(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            [Frozen] Mock<IUnclaimedForgettablePayloadsCleanupJob> cleanupJobMock,
            UnclaimedForgettablePayloadsCleanupBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(50);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();

            cleanupJobMock
                .Setup(job => job.ExecuteAsync(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>()
                .Verifiable();
            
            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * (expectedNumberOfReconciliationJobInvocations + 1) + jobExecutionInterval / 2,
                cancellationToken);

            await reconciliationService.StopAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * expectedNumberOfReconciliationJobInvocations,
                cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            cleanupJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedNumberOfReconciliationJobInvocations));
        }
        
        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteCleanupJobInALoopUntilStoppedEventIfExecutionThrowsException(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IForgettablePayloadConfiguration> configurationMock,
            [Frozen] Mock<IUnclaimedForgettablePayloadsCleanupJob> cleanupJobMock,
            UnclaimedForgettablePayloadsCleanupBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(150);
            configurationMock
                .SetupGet(configuration => configuration.UnclaimedForgettablePayloadsCleanupJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();

            cleanupJobMock
                .Setup(job => job.ExecuteAsync(It.IsAny<CancellationToken>()))
                .Throws<Exception>()
                .Verifiable();
            
            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * (expectedNumberOfReconciliationJobInvocations + 1) + jobExecutionInterval / 2,
                cancellationToken);

            await reconciliationService.StopAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * expectedNumberOfReconciliationJobInvocations,
                cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            cleanupJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.AtLeast(expectedNumberOfReconciliationJobInvocations));
        }
    }
}