using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Configurations;
using EventSourcing.Reconciliation;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Reconciliation
{
    public class ReconciliationBackgroundService_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConfigurationIsNull(
            IReconciliationJob reconciliationJob,
            ILogger<ReconciliationBackgroundService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new ReconciliationBackgroundService(
                null,
                reconciliationJob,
                logger));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ReconciliationJobIsNull(
            IEventSourcingConfiguration configuration,
            ILogger<ReconciliationBackgroundService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new ReconciliationBackgroundService(
                configuration,
                null,
                logger));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_LoggerIsNull(
            IEventSourcingConfiguration configuration,
            IReconciliationJob reconciliationJob)
        {
            Assert.Throws<ArgumentNullException>(() => new ReconciliationBackgroundService(
                configuration,
                reconciliationJob,
                null));
        }

        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventSourcingConfiguration configuration,
            IReconciliationJob reconciliationJob,
            ILogger<ReconciliationBackgroundService> logger)
        {
            _ = new ReconciliationBackgroundService(
                configuration,
                reconciliationJob,
                logger);
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Starting_And_CancellationIsAlreadyRequested(
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IReconciliationJob> reconciliationJobMock,
            ReconciliationBackgroundService reconciliationService)
        {
            var cancellationToken = new CancellationToken(true);

            await reconciliationService.StartAsync(cancellationToken);

            configurationMock.VerifyNoOtherCalls();
            reconciliationJobMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ExecuteReconciliationJob_When_Started(
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IReconciliationJob> reconciliationJobMock,
            ReconciliationBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(250);
            configurationMock
                .SetupGet(configuration => configuration.ReconciliationJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();

            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(jobExecutionInterval, cancellationToken);
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            reconciliationJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteReconciliationJobInALoop_When_Started(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IReconciliationJob> reconciliationJobMock,
            ReconciliationBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(50);
            configurationMock
                .SetupGet(configuration => configuration.ReconciliationJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();

            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * (expectedNumberOfReconciliationJobInvocations + 1) +
                jobExecutionInterval / 2,
                cancellationToken);

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            reconciliationJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedNumberOfReconciliationJobInvocations));
        }

        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteReconciliationJobInALoopUntilStopped_When_Started(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IReconciliationJob> reconciliationJobMock,
            ReconciliationBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(150);
            configurationMock
                .SetupGet(configuration => configuration.ReconciliationJobInterval)
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
            reconciliationJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.AtLeast(expectedNumberOfReconciliationJobInvocations));
        }

        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteReconciliationJobInALoopUntilStoppedEventIfExecutionThrowsOperationCancelledException(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IReconciliationJob> reconciliationJobMock,
            ReconciliationBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(150);
            configurationMock
                .SetupGet(configuration => configuration.ReconciliationJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();

            reconciliationJobMock
                .Setup(job => job.ExecuteAsync(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>()
                .Verifiable();

            await reconciliationService.StartAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * expectedNumberOfReconciliationJobInvocations + jobExecutionInterval / 2,
                cancellationToken);

            await reconciliationService.StopAsync(cancellationToken);

            await Task.Delay(
                jobExecutionInterval * expectedNumberOfReconciliationJobInvocations,
                cancellationToken);

            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            reconciliationJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedNumberOfReconciliationJobInvocations));
        }

        [Theory]
        [AutoMoqWithInlineData(5)]
        internal async Task ExecuteReconciliationJobInALoopUntilStoppedEventIfExecutionThrowsException(
            int expectedNumberOfReconciliationJobInvocations,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IReconciliationJob> reconciliationJobMock,
            ReconciliationBackgroundService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            var jobExecutionInterval = TimeSpan.FromMilliseconds(50);
            configurationMock
                .SetupGet(configuration => configuration.ReconciliationJobInterval)
                .Returns(jobExecutionInterval)
                .Verifiable();

            reconciliationJobMock
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
            reconciliationJobMock.Verify(job => job.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedNumberOfReconciliationJobInvocations));
        }
    }
}
