using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Persistence.Abstractions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests
{
    public class EventStreamPublisher_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_StagingWriterIsNull(
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                null,
                storeWriter,
                busPublisher));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_WriterIsNull(
            IEventStreamStagingWriter storeStagingWriter,
            IEventSourcingBusPublisher busPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                storeStagingWriter,
                null,
                busPublisher));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_BusPublisherIsNull(
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                storeStagingWriter,
                storeWriter,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher)
        {
            _ = new EventStreamPublisher(storeStagingWriter, storeWriter, busPublisher);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Publishing_And_StreamIsNull(
            EventStreamPublisher publisher)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => publisher.PublishAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Publishing_And_StreamContainsNoEntriesToAppend(
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            await publisher.PublishAsync(EventStream.NewEventStream(), CancellationToken.None);
            
            storeStagingWriterMock.VerifyNoOtherCalls();
            storeWriterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task WriteAllEntriesToAppendToStaging_When_Publishing(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            storeStagingWriterMock
                .Verify(writer => writer.WriteAsync(
                        stream.EntriesToAppend,
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task WriteAllEntriesToAppendToStream_When_Publishing(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            storeWriterMock
                .Verify(writer => writer.WriteAsync(
                        stream.EntriesToAppend,
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task PublishEventStoreEntriesToBus_When_Publishing_And_WriteResultIsSuccessful(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.Success);

            await publisher.PublishAsync(stream, CancellationToken.None);

            foreach (var eventStreamEntry in entriesToAppend)
            {
                busPublisherMock.Verify(
                    busPublisher => busPublisher.PublishAsync(
                        eventStreamEntry,
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }
        }

        [Theory]
        [AutoMoqData]
        internal async Task NotMarkStagedEntriesAsPublished_When_Publishing_And_WriteResultIsSuccessful_AndBusPublishingFails(
            Exception busPublishingException,
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.Success);

            busPublisherMock
                .Setup(busPublisher => busPublisher.PublishAsync(It.IsAny<EventStreamEntry>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(busPublishingException);

            await Assert.ThrowsAsync<Exception>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify(
                writer => writer.MarkAsPublishedAsync(
                    stagingId,
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [AutoMoqData]
        internal async Task MarkStagedEntriesAsPublished_When_Publishing_And_WriteResultIsSuccessful(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.Success);

            await publisher.PublishAsync(stream, CancellationToken.None);

            storeStagingWriterMock.Verify(
                writer => writer.MarkAsPublishedAsync(
                    stagingId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_EventStreamOptimisticConcurrencyException_When_Publishing_And_WriteResultIsSequenceAlreadyTaken(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.SequenceAlreadyTaken);

            await Assert.ThrowsAsync<EventStreamOptimisticConcurrencyException>(() => publisher.PublishAsync(stream, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task MarkStagedEventsAsFailedToStore_When_Publishing_And_WriteResultIsSequenceAlreadyTaken(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.SequenceAlreadyTaken);

            await Assert.ThrowsAsync<EventStreamOptimisticConcurrencyException>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify(
                writer => writer.MarkAsFailedToStoreAsync(stagingId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_EventStreamAppendingFailedException_When_Publishing_And_WriteResultIsUnknownFailure(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.UnknownFailure);

            await Assert.ThrowsAsync<EventStreamAppendingFailedException>(() => publisher.PublishAsync(stream, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_Publishing_And_WriteResultIsUnknownFailure(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId)
                .Verifiable();

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.UnknownFailure);

            await Assert.ThrowsAsync<EventStreamAppendingFailedException>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify();
            storeStagingWriterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(EventStreamWriteResult.Undefined)]
        [AutoMoqWithInlineData((EventStreamWriteResult) 99 )] // illegal
        internal async Task Throw_InvalidEnumArgumentException_And_DoNothingWithStagedEvents_When_Publishing_And_WriteResultIsUndefinedOrIllegal(
            EventStreamWriteResult writeResult,
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEntries entriesToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            entriesToAppend = new EventStreamEntries(
                entriesToAppend
                    .Select((entry, i) => new EventStreamEntry(
                        entry.StreamId,
                        entry.EntryId,
                        Convert.ToUInt32(i),
                        entry.EventDescriptor,
                        entry.CausationId,
                        entry.CreationTime,
                        entry.CorrelationId)));
            
            var stream = new EventStream(eventStreamId, EventStreamEntries.Empty);
            stream.AppendEntries(entriesToAppend);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId)
                .Verifiable();

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    stream.EntriesToAppend,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(writeResult)
                .Verifiable();

            await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => publisher.PublishAsync(stream, CancellationToken.None));
            
            storeStagingWriterMock.Verify();
            storeStagingWriterMock.VerifyNoOtherCalls();
        }

        private static async Task PublishAndIgnoreExceptionsAsync(IEventStreamPublisher publisher, EventStream eventStream)
        {
            try
            {
                await publisher.PublishAsync(eventStream, CancellationToken.None);
            }
            catch
            {
                // do nothing
            }
        }
    }
}