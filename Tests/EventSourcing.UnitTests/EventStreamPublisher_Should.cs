using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.EventBus.Abstractions;
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
        public void Throw_ArgumentNullException_When_Creating_And_EventConverterIsNull(
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventStreamBusPublisher streamBusPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                null,
                storeStagingWriter,
                storeWriter,
                streamBusPublisher));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_StagingWriterIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamWriter storeWriter,
            IEventStreamBusPublisher streamBusPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                null,
                storeWriter,
                streamBusPublisher));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_WriterIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamBusPublisher streamBusPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                null,
                streamBusPublisher));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_BusPublisherIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                storeWriter,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventStreamBusPublisher streamBusPublisher)
        {
            _ = new EventStreamPublisher(eventConverter, storeStagingWriter, storeWriter, streamBusPublisher);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_Publishing_And_StreamIsNull(
            EventStreamPublisher publisher)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => publisher.PublishAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task DoNothing_When_Publishing_And_StreamContainsNoEventsToAppend(
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventStreamBusPublisher> streamBusPublisherMock,
            EventStreamPublisher publisher)
        {
            await publisher.PublishAsync(EventStream.NewEventStream(), CancellationToken.None);
            
            eventConverterMock.VerifyNoOtherCalls();
            storeStagingWriterMock.VerifyNoOtherCalls();
            storeWriterMock.VerifyNoOtherCalls();
            streamBusPublisherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task ConvertAllEventsToAppend_When_Publishing(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            foreach (var eventStreamEvent in eventsToAppend)
            {
                eventConverterMock.Verify(
                    converter => converter.ToDescriptorAsync(eventStreamEvent, It.IsAny<CancellationToken>()),
                    Times.Once);
            }
            
            eventConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task WriteAllConvertedEventsToStaging_When_Publishing(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            storeStagingWriterMock
                .Verify(writer => writer.WriteAsync(
                        It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task WriteAllConvertedEventsToStream_When_Publishing(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            storeWriterMock
                .Verify(writer => writer.WriteAsync(
                        It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task PublishAllConvertedEventsToStream_When_Publishing_And_WriteResultIsSuccessful(
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventStreamBusPublisher> streamBusPublisherMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.Success);

            await publisher.PublishAsync(stream, CancellationToken.None);

            streamBusPublisherMock
                .Verify(busPublisher => busPublisher.PublishAsync(
                        It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task MarkStagedEventsAsPublished_After_PublishingAllConvertedEvents_When_Publishing_And_WriteResultIsSuccessful(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventStreamBusPublisher> streamBusPublisherMock,
            EventStreamPublisher publisher)
        {
            var wereConvertedEventsPublished = false;
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.Success);

            streamBusPublisherMock
                .Setup(busPublisher => busPublisher.PublishAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .Callback(() => wereConvertedEventsPublished = true);

            storeStagingWriterMock
                .Setup(writer => writer.MarkAsPublishedAsync(
                    stagingId,
                    It.IsAny<CancellationToken>()))
                .Returns(() => wereConvertedEventsPublished ? Task.CompletedTask : Task.FromException(new Exception()));

            await publisher.PublishAsync(stream, CancellationToken.None);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_EventStreamOptimisticConcurrencyException_When_Publishing_And_WriteResultIsSequenceAlreadyTaken(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.SequenceAlreadyTaken);

            await Assert.ThrowsAsync<EventStreamOptimisticConcurrencyException>(() => publisher.PublishAsync(stream, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task MarkStagedEventsAsFailedToStore_When_Publishing_And_WriteResultIsSequenceAlreadyTaken(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.SequenceAlreadyTaken);

            await Assert.ThrowsAsync<EventStreamOptimisticConcurrencyException>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify(
                writer => writer.MarkAsFailedToStoreAsync(stagingId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_EventStreamAppendingFailedException_When_Publishing_And_WriteResultIsUnknownFailure(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.UnknownFailure);

            await Assert.ThrowsAsync<EventStreamAppendingFailedException>(() => publisher.PublishAsync(stream, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task MarkAsFailedToStoreAsync_When_Publishing_And_WriteResultIsUnknownFailure(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.UnknownFailure);

            await Assert.ThrowsAsync<EventStreamAppendingFailedException>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify(
                writer => writer.MarkAsFailedToStoreAsync(stagingId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [AutoMoqWithInlineData(EventStreamWriteResult.Undefined)]
        [AutoMoqWithInlineData((EventStreamWriteResult) 99 )] // illegal
        public async Task Throw_InvalidEnumArgumentException_And_DoNothingWithStagedEvents_When_Publishing_And_WriteResultIsUndefinedOrIllegal(
            EventStreamWriteResult writeResult,
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            EventStreamEvents eventsToAppend,
            List<EventStreamEventDescriptor> eventDescriptors,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            uint initialSequence = 0;
            eventsToAppend = new EventStreamEvents(
                eventsToAppend
                    .Select(@event => new EventStreamEvent(
                        @event.StreamId,
                        @event.EventId,
                        initialSequence++,
                        @event.Event,
                        @event.EventMetadata)));
            
            var stream = new EventStream(eventStreamId, EventStreamEvents.Empty);
            stream.AppendEvents(eventsToAppend);

            for (var i = 0; i < eventsToAppend.Count; i++)
            {
                var eventToAppend = eventsToAppend[i];
                var eventDescriptor = eventDescriptors[i];
                eventConverterMock
                    .Setup(converter => converter.ToDescriptorAsync(eventToAppend, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventDescriptor);
            }

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId)
                .Verifiable();

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<IReadOnlyList<EventStreamEventDescriptor>>(list => list.SequenceEqual(eventDescriptors)),
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