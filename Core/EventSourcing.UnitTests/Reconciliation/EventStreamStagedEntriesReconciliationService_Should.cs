using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using EventSourcing.Reconciliation;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Reconciliation
{
    public class EventStreamStagedEntriesReconciliationService_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConfigurationIsNull(
            IEventStreamStagingReader stagingReader,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamReader streamReader,
            IEventSourcingBusPublisher busPublisher,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntriesReconciliationService(
                null,
                stagingReader,
                stagingWriter,
                streamReader,
                busPublisher,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_StagingReaderIsNull(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamReader streamReader,
            IEventSourcingBusPublisher busPublisher,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntriesReconciliationService(
                configuration,
                null,
                stagingWriter,
                streamReader,
                busPublisher,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_StagingWriterIsNull(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingReader stagingReader,
            IEventStreamReader streamReader,
            IEventSourcingBusPublisher busPublisher,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntriesReconciliationService(
                configuration,
                stagingReader,
                null,
                streamReader,
                busPublisher,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_StreamReaderIsNull(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingReader stagingReader,
            IEventStreamStagingWriter stagingWriter,
            IEventSourcingBusPublisher busPublisher,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntriesReconciliationService(
                configuration,
                stagingReader,
                stagingWriter,
                null,
                busPublisher,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_BusPublisherIsNull(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingReader stagingReader,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamReader streamReader,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntriesReconciliationService(
                configuration,
                stagingReader,
                stagingWriter,
                streamReader,
                null,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_LoggerIsNull(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingReader stagingReader,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamReader streamReader,
            IEventSourcingBusPublisher busPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntriesReconciliationService(
                configuration,
                stagingReader,
                stagingWriter,
                streamReader,
                busPublisher,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingReader stagingReader,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamReader streamReader,
            IEventSourcingBusPublisher busPublisher,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            _ = new EventStreamStagedEntriesReconciliationService(
                configuration,
                stagingReader,
                stagingWriter,
                streamReader,
                busPublisher,
                logger);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_TryingToReconcileStagedEntriesAsync_And_StagedEntriesParameterIsNull(
            EventStreamStagedEntriesReconciliationService reconciliationService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => reconciliationService.TryToReconcileStagedEntriesAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_TryingToReconcileStagedEntriesAsync_And_TimeSinceStagingIsLessThanConfiguredGracePeriodAfterStagingTime(
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value + TimeSpan.FromMinutes(1))
                .Verifiable();
            
            await reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_TryingToReconcileStagedEntriesAsync_And_ThereAreNoStoredEntriesWithinProvidedSequenceRange(
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(EventStreamEntries.Empty)
                .Verifiable();
            
            await reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowInvalidOperationException_When_TryingToReconcileStagedEntriesAsync_And_ThereAreMoreStoredEntriesWithinProvidedSequenceRangeThanStagedEvents(
            EventStreamStagedEntries stagedEntries,
            EventStreamEventDescriptor extraEventDescriptor,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            var storedEntriesToReturn = stagedEntries.Entries.ToList();
            storedEntriesToReturn.Add(new EventStreamEntry(
                stagedEntries.Entries.StreamId,
                EventStreamEntryId.NewEventStreamEntryId(), 
                stagedEntries.Entries.MaximumSequence + 1,
                extraEventDescriptor,
                EventStreamEntryCausationId.Current, 
                EventStreamEntryCreationTime.Now(), 
                EventStreamEntryCorrelationId.Current));

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(new EventStreamEntries(storedEntriesToReturn))
                .Verifiable();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken));
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task MarkStagedEventsAsFailedToStore_When_TryingToReconcileStagedEntriesAsync_And_ThereAreLessStoredEntriesWithinProvidedSequenceRangeThanStagedEvents(
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            var storedEntriesToReturn = stagedEntries.Entries
                .Take(stagedEntries.Entries.Count - 1)
                .ToList();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(new EventStreamEntries(storedEntriesToReturn))
                .Verifiable();
            
            await reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.Verify(
                writer => writer.MarkAsFailedToStoreAsync(stagedEntries.StagingId, cancellationToken),
                Times.Once);
            
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task MarkStagedEventsAsFailedToStore_When_TryingToReconcileStagedEntriesAsync_And_AtLeastOneStagedEventIsDifferentThanStoredEvent(
            int differentStoredEventIndex,
            EventStreamEntryId differentEntryId,
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            var storedEntriesToReturn = new List<EventStreamEntry>();
            for (var i = 0; i < stagedEntries.Entries.Count; i++)
            {
                if (i == differentStoredEventIndex)
                {
                    var differentEntry = new EventStreamEntry(
                        stagedEntries.Entries[i].StreamId,
                        differentEntryId,
                        stagedEntries.Entries[i].EntrySequence,
                        stagedEntries.Entries[i].EventDescriptor,
                        stagedEntries.Entries[i].CausationId,
                        stagedEntries.Entries[i].CreationTime,
                        stagedEntries.Entries[i].CorrelationId);
                 
                    storedEntriesToReturn.Add(differentEntry);
                }
                else
                {
                    storedEntriesToReturn.Add(stagedEntries.Entries[i]);
                }
            }

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(new EventStreamEntries(storedEntriesToReturn))
                .Verifiable();
            
            await reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.Verify(
                writer => writer.MarkAsFailedToStoreAsync(stagedEntries.StagingId, cancellationToken),
                Times.Once);
            
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_TryingToReconcileStagedEntriesAsync_And_AllStagedEventsAreSameAsStoredEventsButWhenReadingUnmarkedStagedEventByIdNullIsReturned(
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(stagedEntries.Entries)
                .Verifiable();

            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(
                    stagedEntries.StagingId,
                    cancellationToken))
                .ReturnsAsync(null as EventStreamStagedEntries)
                .Verifiable();
            
            await reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowInvalidOperationException_When_TryingToReconcileStagedEntriesAsync_And_DifferentStagedEntriesAreReturnedWhenReadingUnmarkedStagedEventById(
            EventStreamStagedEntries stagedEntries,
            EventStreamStagedEntries differentStagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService,
            CancellationToken cancellationToken)
        {
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(stagedEntries.Entries)
                .Verifiable();

            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(
                    stagedEntries.StagingId,
                    cancellationToken))
                .ReturnsAsync(differentStagedEntries)
                .Verifiable();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken));
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task NotMarkStagedEntriesAsPublished_When_TryingToReconcileStagedEntriesAsync_And_PublishingToBusThrowsException(
            int throwOnPublishingEntryIndex,
            Exception exception,
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(stagedEntries.Entries)
                .Verifiable();

            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(
                    stagedEntries.StagingId,
                    cancellationToken))
                .ReturnsAsync(stagedEntries)
                .Verifiable();

            for (var i = 0; i < stagedEntries.Entries.Count; i++)
            {
                var entryToPublish = stagedEntries.Entries[i];
                var publishEntrySetup = busPublisherMock
                    .Setup(publisher => publisher.PublishAsync(entryToPublish, cancellationToken));

                if (throwOnPublishingEntryIndex == i)
                {
                    publishEntrySetup
                        .Throws(exception)
                        .Verifiable();
                    
                    break;
                }

                publishEntrySetup
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            }

            var thrownException = await Assert.ThrowsAsync<Exception>(() =>
                reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken));

            Assert.Equal(exception, thrownException);
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.Verify();
            busPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task MarkStagedEntriesAsPublished_When_TryingToReconcileStagedEntriesAsync_And_PublishingToBusIsSuccessful(
            EventStreamStagedEntries stagedEntries,
            [Frozen] Mock<IEventSourcingConfiguration> configurationMock,
            [Frozen] Mock<IEventStreamStagingReader> stagingReaderMock,
            [Frozen] Mock<IEventStreamStagingWriter> stagingWriterMock,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamStagedEntriesReconciliationService reconciliationService)
        {
            var cancellationToken = CancellationToken.None;
            configurationMock
                .Setup(configuration => configuration.ReconciliationJobGracePeriodAfterStagingTime)
                .Returns(DateTime.UtcNow - stagedEntries.StagingTime.Value - TimeSpan.FromMinutes(1))
                .Verifiable();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken))
                .ReturnsAsync(stagedEntries.Entries)
                .Verifiable();

            stagingReaderMock
                .Setup(reader => reader.ReadUnmarkedStagedEntriesAsync(
                    stagedEntries.StagingId,
                    cancellationToken))
                .ReturnsAsync(stagedEntries)
                .Verifiable();

            for (var i = 0; i < stagedEntries.Entries.Count; i++)
            {
                busPublisherMock
                    .Setup(publisher => publisher.PublishAsync(stagedEntries.Entries[i], cancellationToken))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            }

            await reconciliationService.TryToReconcileStagedEntriesAsync(stagedEntries, cancellationToken);
            
            configurationMock.Verify();
            configurationMock.VerifyNoOtherCalls();
            stagingReaderMock.Verify();
            stagingReaderMock.VerifyNoOtherCalls();
            stagingWriterMock.Verify(writer => writer.MarkAsPublishedAsync(stagedEntries.StagingId, cancellationToken), Times.Once);
            stagingWriterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();
            busPublisherMock.Verify();
            busPublisherMock.VerifyNoOtherCalls();
        }
    }
}