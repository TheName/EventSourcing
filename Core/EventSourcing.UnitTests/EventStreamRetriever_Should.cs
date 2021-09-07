using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests
{
    public class EventStreamRetriever_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamReader(
            IEventStreamEventConverter eventConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamRetriever(null, eventConverter));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventConverter(
            IEventStreamReader eventStreamReader)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamRetriever(eventStreamReader, null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNotNullParameters(
            IEventStreamReader eventStreamReader,
            IEventStreamEventConverter eventConverter)
        {
            _ = new EventStreamRetriever(eventStreamReader, eventConverter);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ConvertAllReadEntries_When_Retrieving(
            [Frozen] EventStreamId streamId,
            CancellationToken cancellationToken,
            EventStreamEntries entries,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            EventStreamRetriever retriever)
        {
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId)));
            
            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(streamId, cancellationToken))
                .ReturnsAsync(entries);

            _ = await retriever.RetrieveAsync(streamId, cancellationToken);

            foreach (var eventStreamEntry in entries)
            {
                eventConverterMock.Verify(converter => converter.FromEventDescriptor(eventStreamEntry.EventDescriptor), Times.Once);
            }
            
            eventConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnEventStreamWithCorrectStreamId_When_Retrieving(
            [Frozen] EventStreamId streamId,
            CancellationToken cancellationToken,
            EventStreamEntries entries,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            EventStreamRetriever retriever)
        {
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId)));
            
            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(streamId, cancellationToken))
                .ReturnsAsync(entries);

            var stream = await retriever.RetrieveAsync(streamId, cancellationToken);

            Assert.Equal(streamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnEventStreamWithCorrectMaxSequence_When_Retrieving(
            [Frozen] EventStreamId streamId,
            CancellationToken cancellationToken,
            EventStreamEntries entries,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            EventStreamRetriever retriever)
        {
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId)));
            
            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(streamId, cancellationToken))
                .ReturnsAsync(entries);

            var stream = await retriever.RetrieveAsync(streamId, cancellationToken);

            Assert.Equal<uint>(entries.Max(entry => (uint) entry.EntrySequence), stream.MaxSequence);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnConvertedEntries_When_Retrieving(
            [Frozen] EventStreamId streamId,
            CancellationToken cancellationToken,
            List<(EventStreamEntry Entry, object Event)> entriesWithEvents,
            [Frozen] Mock<IEventStreamReader> eventStreamReaderMock,
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            EventStreamRetriever retriever)
        {
            entriesWithEvents = entriesWithEvents
                .Select((tuple, i) => (
                    new EventStreamEntry(
                        tuple.Entry.StreamId,
                        tuple.Entry.EntryId,
                        Convert.ToUInt32(i),
                        tuple.Entry.EventDescriptor,
                        tuple.Entry.CausationId,
                        tuple.Entry.CreationTime,
                        tuple.Entry.CorrelationId),
                    tuple.Event))
                .ToList();

            eventStreamReaderMock
                .Setup(reader => reader.ReadAsync(streamId, cancellationToken))
                .ReturnsAsync(new EventStreamEntries(entriesWithEvents.Select(tuple => tuple.Entry)))
                .Verifiable();

            foreach (var (entry, @event) in entriesWithEvents)
            {
                eventConverterMock
                    .Setup(converter => converter.FromEventDescriptor(entry.EventDescriptor))
                    .Returns(@event)
                    .Verifiable();
            }

            var stream = await retriever.RetrieveAsync(streamId, cancellationToken);
            
            eventConverterMock.Verify();
            eventConverterMock.VerifyNoOtherCalls();
            eventStreamReaderMock.Verify();
            eventStreamReaderMock.VerifyNoOtherCalls();

            for (var i = 0; i < entriesWithEvents.Count; i++)
            {
                var entryWithEvent = entriesWithEvents[i];
                var eventWithMetadata = stream.EventsWithMetadata[i];
                
                Assert.Equal(entryWithEvent.Event, eventWithMetadata.Event);
                Assert.Equal(entryWithEvent.Entry.ToEventMetadata(), eventWithMetadata.EventMetadata);
            }
        }
    }
}