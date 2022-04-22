using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Reconciliation;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using EventSourcing.Reconciliation;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Reconciliation
{
    public class ReconciliationJob_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_StagingReaderIsNull(
            IEventStreamStagedEntriesReconciliationService reconciliationService,
            ILogger<ReconciliationJob> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new ReconciliationJob(
                null,
                reconciliationService,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ReconciliationServiceIsNull(
            IEventStreamStagingReader stagingReader,
            ILogger<ReconciliationJob> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new ReconciliationJob(
                stagingReader,
                null,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_LoggerIsNull(
            IEventStreamStagingReader stagingReader,
            IEventStreamStagedEntriesReconciliationService reconciliationService)
        {
            Assert.Throws<ArgumentNullException>(() => new ReconciliationJob(
                stagingReader,
                reconciliationService,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventStreamStagingReader stagingReader,
            IEventStreamStagedEntriesReconciliationService reconciliationService,
            ILogger<ReconciliationJob> logger)
        {
            _ = new ReconciliationJob(
                stagingReader,
                reconciliationService,
                logger);
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Executing_And_StagingReaderReturnsNull(
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as IReadOnlyCollection<EventStreamStagedEntries>)
                .Verifiable();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                reconciliationJob.ExecuteAsync(CancellationToken.None));
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Executing_And_StagingReaderReturnsEmptyStagedEntriesCollection(
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventStreamStagedEntries>())
                .Verifiable();

            await reconciliationJob.ExecuteAsync(CancellationToken.None);
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryToReconcileEveryStagedEntry_When_Executing(
            IReadOnlyCollection<EventStreamStagedEntries> stagedEntriesCollection,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            var cancellationToken = CancellationToken.None;
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(cancellationToken))
                .ReturnsAsync(stagedEntriesCollection)
                .Verifiable();

            foreach (var stagedEntries in stagedEntriesCollection)
            {
                reconciliationServiceMock
                    .Setup(service => service.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            }

            await reconciliationJob.ExecuteAsync(cancellationToken);
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.Verify();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryToReconcileEveryStagedEntry_When_Executing_And_ReconciliationServiceThrowsOperationCancelledException(
            IReadOnlyCollection<EventStreamStagedEntries> stagedEntriesCollection,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            var cancellationToken = CancellationToken.None;
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(cancellationToken))
                .ReturnsAsync(stagedEntriesCollection)
                .Verifiable();

            foreach (var stagedEntries in stagedEntriesCollection)
            {
                reconciliationServiceMock
                    .Setup(service => service.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken))
                    .Throws<OperationCanceledException>()
                    .Verifiable();
            }

            await reconciliationJob.ExecuteAsync(cancellationToken);
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.Verify();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryToReconcileEveryStagedEntry_When_Executing_And_ReconciliationServiceThrowsException(
            IReadOnlyCollection<EventStreamStagedEntries> stagedEntriesCollection,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            var cancellationToken = CancellationToken.None;
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(cancellationToken))
                .ReturnsAsync(stagedEntriesCollection)
                .Verifiable();

            foreach (var stagedEntries in stagedEntriesCollection)
            {
                reconciliationServiceMock
                    .Setup(service => service.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken))
                    .Throws<Exception>()
                    .Verifiable();
            }

            await reconciliationJob.ExecuteAsync(cancellationToken);
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.Verify();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        internal async Task StopTryingToReconcileStagedEntries_When_Executing_And_ReconciliationServiceThrowsOperationCanceledException_And_CancellationIsRequested(
            int indexWhenOperationCancelledExceptionIsThrown,
            List<EventStreamStagedEntries> stagedEntriesCollection,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            var cancellationToken = new CancellationToken(true);
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(cancellationToken))
                .ReturnsAsync(stagedEntriesCollection)
                .Verifiable();

            for (var i = 0; i < stagedEntriesCollection.Count; i++)
            {
                var stagedEntries = stagedEntriesCollection[i];
                var reconciliationSetup = reconciliationServiceMock
                    .Setup(service => service.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken));

                if (i == indexWhenOperationCancelledExceptionIsThrown)
                {
                    reconciliationSetup
                        .Throws<OperationCanceledException>()
                        .Verifiable();
                    
                    break;
                }

                reconciliationSetup
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            }

            await Assert.ThrowsAsync<OperationCanceledException>(() => reconciliationJob.ExecuteAsync(cancellationToken));
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.Verify();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryToReconcileEveryStagedEntry_When_Executing_And_ReconciliationServiceThrowsOperationCanceledException_And_CancellationIsRequested(
            IReadOnlyCollection<EventStreamStagedEntries> stagedEntriesCollection,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagedEntriesReconciliationService> reconciliationServiceMock,
            ReconciliationJob reconciliationJob)
        {
            var cancellationToken = new CancellationToken(true);
            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(cancellationToken))
                .ReturnsAsync(stagedEntriesCollection)
                .Verifiable();

            foreach (var stagedEntries in stagedEntriesCollection)
            {
                reconciliationServiceMock
                    .Setup(service => service.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken))
                    .Throws<Exception>()
                    .Verifiable();
            }

            await reconciliationJob.ExecuteAsync(cancellationToken);
            
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            reconciliationServiceMock.Verify();
            reconciliationServiceMock.VerifyNoOtherCalls();
        }
    }
}