using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus;
using EventSourcing.Conversion;
using EventSourcing.Exceptions;
using EventSourcing.Hooks;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Enums;
using EventSourcing.Persistence.ValueObjects;
using EventSourcing.ValueObjects;
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
            IEventStreamStagingWriter stagingWriter,
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher,
            IEnumerable<IEventStreamEventWithMetadataPrePublishingHook> prePublishingHooks)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                null,
                stagingWriter,
                storeWriter,
                busPublisher,
                prePublishingHooks));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_StagingWriterIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher,
            IEnumerable<IEventStreamEventWithMetadataPrePublishingHook> prePublishingHooks)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                null,
                storeWriter,
                busPublisher,
                prePublishingHooks));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_WriterIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventSourcingBusPublisher busPublisher,
            IEnumerable<IEventStreamEventWithMetadataPrePublishingHook> prePublishingHooks)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                null,
                busPublisher,
                prePublishingHooks));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_BusPublisherIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEnumerable<IEventStreamEventWithMetadataPrePublishingHook> prePublishingHooks)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                storeWriter,
                null,
                prePublishingHooks));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_PrePublishingHookCollectionIsNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                storeWriter,
                busPublisher,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher,
            IEnumerable<IEventStreamEventWithMetadataPrePublishingHook> prePublishingHooks)
        {
            _ = new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                storeWriter,
                busPublisher,
                prePublishingHooks);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_PrePublishingHookCollectionIsEmpty(
            IEventStreamEventConverter eventConverter,
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventSourcingBusPublisher busPublisher)
        {
            _ = new EventStreamPublisher(
                eventConverter,
                storeStagingWriter,
                storeWriter,
                busPublisher,
                new List<IEventStreamEventWithMetadataPrePublishingHook>());
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
        internal async Task DoNothing_When_Publishing_And_StreamContainsNoEventsToAppend(
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            var stream = new PublishableEventStream(new AppendableEventStream(EventStream.NewEventStream()));

            await publisher.PublishAsync(stream, CancellationToken.None);

            storeStagingWriterMock.VerifyNoOtherCalls();
            storeWriterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CallAllPrePublishingHooksWithAllEventsBeforeConvertingEvents_When_Publishing(
            EventStreamId eventStreamId,
            List<object> eventsToAppend,
            [Frozen] Mock<IEventStreamEventConverter> eventStreamEventConverterMock,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamWriter streamWriter,
            IEventSourcingBusPublisher busPublisher,
            List<Mock<IEventStreamEventWithMetadataPrePublishingHook>> prePublishingHookMocks)
        {
            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventToAppend in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventToAppend);
            }

            foreach (var prePublishingModifierMock in prePublishingHookMocks)
            {
                foreach (var eventStreamEventWithMetadata in appendableStream.EventsWithMetadataToAppend)
                {
                    prePublishingModifierMock
                        .Setup(modifier =>
                            modifier.PreEventStreamEventWithMetadataPublishHookAsync(eventStreamEventWithMetadata, It.IsAny<CancellationToken>()))
                        .Returns(() =>
                        {
                            eventStreamEventConverterMock.VerifyNoOtherCalls();
                            return Task.CompletedTask;
                        })
                        .Verifiable();
                }
            }

            var stream = new PublishableEventStream(appendableStream);

            var publisher = new EventStreamPublisher(
                eventStreamEventConverterMock.Object,
                stagingWriter,
                streamWriter,
                busPublisher,
                prePublishingHookMocks.Select(mock => mock.Object));

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            foreach (var prePublishingModifierMock in prePublishingHookMocks)
            {
                prePublishingModifierMock.Verify();
                prePublishingModifierMock.VerifyNoOtherCalls();
            }

            foreach (var eventToAppend in eventsToAppend)
            {
                eventStreamEventConverterMock.Verify(converter => converter.ToEventDescriptor(eventToAppend), Times.Once);
            }
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task NotCallConverter_When_And_AtLeastOneOfPrePublishingHooksThrows(
            int indexOfThrowingModifier,
            Exception thrownException,
            EventStreamId eventStreamId,
            List<object> eventsToAppend,
            [Frozen] Mock<IEventStreamEventConverter> eventStreamEventConverterMock,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamWriter streamWriter,
            IEventSourcingBusPublisher busPublisher,
            List<Mock<IEventStreamEventWithMetadataPrePublishingHook>> prePublishingHookMocks)
        {
            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventToAppend in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventToAppend);
            }

            for (var i = 0; i < prePublishingHookMocks.Count; i++)
            {
                var mock = prePublishingHookMocks[i];
                var modifierMockSetup = mock
                    .Setup(modifier =>
                        modifier.PreEventStreamEventWithMetadataPublishHookAsync(It.IsAny<EventStreamEventWithMetadata>(), It.IsAny<CancellationToken>()));

                modifierMockSetup
                    .Returns(i == indexOfThrowingModifier
                        ? Task.FromException(thrownException)
                        : Task.CompletedTask)
                    .Verifiable();
            }

            var stream = new PublishableEventStream(appendableStream);

            var publisher = new EventStreamPublisher(
                eventStreamEventConverterMock.Object,
                stagingWriter,
                streamWriter,
                busPublisher,
                prePublishingHookMocks.Select(mock => mock.Object));

            var aggregateException = await Assert.ThrowsAsync<AggregateException>(() => publisher.PublishAsync(stream, CancellationToken.None));

            Assert.Equal(eventsToAppend.Count, aggregateException.InnerExceptions.Count);
            Assert.All(aggregateException.InnerExceptions, exception => Assert.Equal(thrownException, exception));

            foreach (var prePublishingModifierMock in prePublishingHookMocks)
            {
                prePublishingModifierMock.Verify();
                prePublishingModifierMock.VerifyNoOtherCalls();
            }

            eventStreamEventConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ConvertAllEventsToAppend_When_Publishing(
            EventStreamId eventStreamId,
            List<object> eventsToAppend,
            [Frozen] Mock<IEventStreamEventConverter> eventStreamEventConverterMock,
            EventStreamPublisher publisher)
        {
            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventToAppend in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventToAppend);
            }

            var stream = new PublishableEventStream(appendableStream);

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            foreach (var eventToAppend in eventsToAppend)
            {
                eventStreamEventConverterMock.Verify(converter => converter.ToEventDescriptor(eventToAppend), Times.Once);
            }
        }

        [Theory]
        [AutoMoqData]
        internal async Task WriteAllConvertedEventsToAppendToStaging_When_Publishing(
            EventStreamId eventStreamId,
            List<(object Event, EventStreamEventDescriptor EventDescriptor)> eventsToAppend,
            [Frozen] Mock<IEventStreamEventConverter> eventStreamEventConverterMock,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            EventStreamPublisher publisher)
        {
            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            var appendedEventsMetadata = new List<EventStreamEventWithMetadata>();
            foreach (var (@event, eventDescriptor) in eventsToAppend)
            {
                var eventWithMetadata = appendableStream.AppendEventWithMetadata(@event);
                appendedEventsMetadata.Add(eventWithMetadata);

                eventStreamEventConverterMock
                    .Setup(converter => converter.ToEventDescriptor(@event))
                    .Returns(eventDescriptor);
            }

            var stream = new PublishableEventStream(appendableStream);

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            var assertEntries = new Func<EventStreamEntries, bool>(entries =>
            {
                for (var i = 0; i < appendedEventsMetadata.Count; i++)
                {
                    var expectedEventMetadata = appendedEventsMetadata[i].EventMetadata;
                    var expectedEventDescriptor = eventsToAppend[i].EventDescriptor;

                    var actualEventMetadata = entries[i].ToEventMetadata();
                    var actualEventDescriptor = entries[i].EventDescriptor;

                    Assert.Equal(expectedEventMetadata, actualEventMetadata);
                    Assert.Equal(expectedEventDescriptor, actualEventDescriptor);
                }

                return true;
            });

            storeStagingWriterMock
                .Verify(writer => writer.WriteAsync(
                        It.Is<EventStreamEntries>(entries => assertEntries(entries)),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task WriteAllConvertedEventsToAppendToStream_When_Publishing(
            EventStreamId eventStreamId,
            List<(object Event, EventStreamEventDescriptor EventDescriptor)> eventsToAppend,
            [Frozen] Mock<IEventStreamEventConverter> eventStreamEventConverterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            var appendedEventsMetadata = new List<EventStreamEventWithMetadata>();
            foreach (var (@event, eventDescriptor) in eventsToAppend)
            {
                var eventWithMetadata = appendableStream.AppendEventWithMetadata(@event);
                appendedEventsMetadata.Add(eventWithMetadata);

                eventStreamEventConverterMock
                    .Setup(converter => converter.ToEventDescriptor(@event))
                    .Returns(eventDescriptor);
            }

            var stream = new PublishableEventStream(appendableStream);

            await PublishAndIgnoreExceptionsAsync(publisher, stream);

            var assertEntries = new Func<EventStreamEntries, bool>(entries =>
            {
                for (var i = 0; i < appendedEventsMetadata.Count; i++)
                {
                    var expectedEventMetadata = appendedEventsMetadata[i].EventMetadata;
                    var expectedEventDescriptor = eventsToAppend[i].EventDescriptor;

                    var actualEventMetadata = entries[i].ToEventMetadata();
                    var actualEventDescriptor = entries[i].EventDescriptor;

                    Assert.Equal(expectedEventMetadata, actualEventMetadata);
                    Assert.Equal(expectedEventDescriptor, actualEventDescriptor);
                }

                return true;
            });

            storeWriterMock
                .Verify(writer => writer.WriteAsync(
                        It.Is<EventStreamEntries>(entries => assertEntries(entries)),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task PublishEventStoreEntriesToBus_When_Publishing_And_WriteResultIsSuccessful(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            EventStreamEntries stagedEntries = null;
            EventStreamEntries storedEntries = null;

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .Callback<EventStreamEntries, CancellationToken>((eventStreamEntries, _) =>
                {
                    stagedEntries = eventStreamEntries;
                })
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .Callback<EventStreamEntries, CancellationToken>((eventStreamEntries, _) =>
                {
                    storedEntries = eventStreamEntries;
                })
                .ReturnsAsync(EventStreamWriteResult.Success);

            await publisher.PublishAsync(stream, CancellationToken.None);

            Assert.Equal(stagedEntries, storedEntries);
            Assert.All(stagedEntries,
                entry => busPublisherMock.Verify(
                    busPublisher => busPublisher.PublishAsync(entry, It.IsAny<CancellationToken>()), Times.Once));
        }

        [Theory]
        [AutoMoqData]
        internal async Task MarkStagedEntriesAsPublished_When_Publishing_And_WriteResultIsSuccessful_And_BusPublishingIsSuccessful(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            EventStreamEntries stagedEntries = null;

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .Callback<EventStreamEntries, CancellationToken>((eventStreamEntries, _) =>
                {
                    stagedEntries = eventStreamEntries;
                })
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.Is<EventStreamEntries>(entries => entries == stagedEntries),
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
        internal async Task NotMarkStagedEntriesAsPublished_When_Publishing_And_WriteResultIsSuccessful_And_BusPublishingFails(
            Exception busPublishingException,
            [Frozen] EventStreamId eventStreamId,
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            [Frozen] Mock<IEventSourcingBusPublisher> busPublisherMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.Success);

            busPublisherMock
                .Setup(busPublisher => busPublisher.PublishAsync(It.IsAny<EventStreamEntry>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(busPublishingException);

            await Assert.ThrowsAsync<Exception>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify(
                writer => writer.MarkAsPublishedAsync(
                    It.IsAny<EventStreamStagingId>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_EventStreamOptimisticConcurrencyException_When_Publishing_And_WriteResultIsSequenceAlreadyTaken(
            [Frozen] EventStreamId eventStreamId,
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.SequenceAlreadyTaken);

            await Assert.ThrowsAsync<EventStreamOptimisticConcurrencyException>(() => publisher.PublishAsync(stream, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task MarkStagedEventsAsFailedToStore_When_Publishing_And_WriteResultIsSequenceAlreadyTaken(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
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
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStreamWriteResult.UnknownFailure);

            await Assert.ThrowsAsync<EventStreamAppendingFailedException>(() => publisher.PublishAsync(stream, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothingWithStagedEntries_When_Publishing_And_WriteResultIsUnknownFailure(
            EventStreamStagingId stagingId,
            [Frozen] EventStreamId eventStreamId,
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId)
                .Verifiable();

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
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
            List<EventStreamEventWithMetadata> eventsToAppend,
            [Frozen] Mock<IEventStreamStagingWriter> storeStagingWriterMock,
            [Frozen] Mock<IEventStreamWriter> storeWriterMock,
            EventStreamPublisher publisher)
        {
            eventsToAppend = eventsToAppend
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            var appendableStream = new AppendableEventStream(EventStream.NewEventStream(eventStreamId));
            foreach (var eventStreamEventWithMetadata in eventsToAppend)
            {
                appendableStream.AppendEventWithMetadata(eventStreamEventWithMetadata);
            }

            var stream = new PublishableEventStream(appendableStream);

            storeStagingWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stagingId)
                .Verifiable();

            storeWriterMock
                .Setup(writer => writer.WriteAsync(
                    It.IsAny<EventStreamEntries>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(writeResult)
                .Verifiable();

            await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => publisher.PublishAsync(stream, CancellationToken.None));

            storeStagingWriterMock.Verify();
            storeStagingWriterMock.VerifyNoOtherCalls();
        }

        private static async Task PublishAndIgnoreExceptionsAsync(IEventStreamPublisher publisher, PublishableEventStream eventStream)
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
