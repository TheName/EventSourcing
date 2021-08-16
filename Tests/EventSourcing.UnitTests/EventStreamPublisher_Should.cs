using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
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
            IEventStreamWriter storeWriter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                null,
                storeWriter));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_WriterIsNull(
            IEventStreamStagingWriter storeStagingWriter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                storeStagingWriter,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter)
        {
            _ = new EventStreamPublisher(storeStagingWriter, storeWriter);
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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
                    .Select((@event, i) => new EventStreamEntry(
                        @event.StreamId,
                        @event.EntryId,
                        Convert.ToUInt32(i++),
                        @event.EventDescriptor,
                        @event.EntryMetadata)));
            
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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
                        Convert.ToUInt32(i++),
                        entry.EventDescriptor,
                        entry.EntryMetadata)));
            
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