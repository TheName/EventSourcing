using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStream_Should
    {
        [Fact]
        public void CreateNewEventStreamWithEmptyEntries_When_CallingNewEventStream()
        {
            var stream = EventStream.NewEventStream();
            Assert.NotNull(stream.StreamId);
            Assert.Empty(stream.EventsWithMetadata);
            Assert.Empty(stream.EventsWithMetadataToAppend);
        }
        
        [Theory]
        [AutoMoqData]
        public void CreateNewEventStreamWithProvidedStreamIdAndEmptyEntries_When_CallingNewEventStream(EventStreamId streamId)
        {
            var stream = EventStream.NewEventStream(streamId);
            Assert.Equal(streamId, stream.StreamId);
            Assert.Empty(stream.EventsWithMetadata);
            Assert.Empty(stream.EventsWithMetadataToAppend);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullStreamId(
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(
                null,
                eventsWithMetadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventsWithMetadata(
            EventStreamId streamId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(
                streamId,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_CreatingWithDifferentStreamIdThanAssignedToEventsWithMetadata(
            EventStreamId streamId,
            EventStreamId streamIdAssignedToEventsWithMetadata,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamIdAssignedToEventsWithMetadata,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));
            
            Assert.Throws<InvalidEventStreamIdException>(() => new EventStream(
                streamId, 
                eventsWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_CreatingWithSameStreamIdAsAssignedToEventsWithMetadata_And_EventsWithMetadataSequenceDoesNotStartWithZero(
            EventStreamId streamId,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i + 1),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));
            
            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => new EventStream(
                streamId,
                eventsWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullStreamIdAndEmptyEventsWithMetadata(
            EventStreamId streamId)
        {
            _ = new EventStream(streamId, new List<EventStreamEventWithMetadata>());
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithSameStreamIdAsAssignedToEventsWithMetadata_And_EventsWithMetadataSequenceStartsWithZero(
            EventStreamId streamId,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            _ = new EventStream(streamId, eventsWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(EventStreamId streamId)
        {
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>());

            Assert.Equal(streamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsWithMetadataProvidedDuringCreation_When_GettingEventsWithMetadata(
            EventStreamId streamId,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream = new EventStream(streamId, eventsWithMetadata);

            Assert.NotSame(eventsWithMetadata, stream.EventsWithMetadata);
            Assert.Null(stream.EventsWithMetadata as List<EventStreamEventWithMetadata>);
            Assert.False(ReferenceEquals(eventsWithMetadata, stream.EventsWithMetadata));
            Assert.Equal(eventsWithMetadata, stream.EventsWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEmptyCollection_When_GettingEntriesToAppend(
            EventStreamId streamId,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream = new EventStream(streamId, eventsWithMetadata);

            Assert.Empty(stream.EventsWithMetadataToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastEventSequenceIncreasedByOne_When_GettingNextSequence(
            EventStreamId streamId,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream = new EventStream(streamId, eventsWithMetadata);

            Assert.Equal<uint>(eventsWithMetadata.Last().EventMetadata.EntrySequence + 1, stream.NextSequence);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AppendingEventsWithMetadata_And_ProvidedEventsWithMetadataIsNull(
            EventStream stream)
        {
            Assert.Throws<ArgumentNullException>(() => stream.AppendEventsWithMetadata(null));
        }

        [Fact]
        public void Throw_ArgumentNullException_When_AppendingEventsWithMetadata_And_ProvidedEventsWithMetadataIsNull_And_EventStreamIsEmpty()
        {
            var stream = EventStream.NewEventStream();
            Assert.Throws<ArgumentNullException>(() => stream.AppendEventsWithMetadata(null));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEventsWithMetadata_And_AtLeastOneOfEventsWithMetadataHasInvalidStreamId_And_AllEventsWithMetadataHaveValidSequence(
            int invalidStreamIdIndex,
            EventStream stream,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        i == invalidStreamIdIndex ? eventWithMetadata.EventMetadata.StreamId : validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEventsWithMetadata(eventsWithMetadata));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEventsWithMetadata_And_AtLeastOneOfEventsWithMetadataHasInvalidStreamId_And_AllEventsWithMetadataHaveValidSequence_And_EventStreamIsEmpty(
            int invalidStreamIdIndex,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        i == invalidStreamIdIndex ? eventWithMetadata.EventMetadata.StreamId : validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEventsWithMetadata(eventsWithMetadata));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEventsWithMetadata_And_AllEventsWithMetadataHaveValidStreamId_And_AtLeastOneOfEventsWithMetadataHasInvalidSequence(
            int invalidSequenceIndex,
            EventStream stream,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextSequence = stream.NextSequence;
            var getNextSequenceFunc = new Func<int, EventStreamEventMetadata, EventStreamEntrySequence>(
                (currentIndex, eventMetadata) =>
                {
                    var currentValidSequence = nextSequence++;
                    Assert.NotEqual<uint>(eventMetadata.EntrySequence, currentValidSequence);
                    return currentIndex == invalidSequenceIndex ? eventMetadata.EntrySequence : currentValidSequence;
                });
            
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        getNextSequenceFunc(i, eventWithMetadata.EventMetadata),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEventsWithMetadata(eventsWithMetadata));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEventsWithMetadata_And_AllEventsWithMetadataHaveValidStreamId_And_AtLeastOneOfEventsWithMetadataHasInvalidSequence_And_EventStreamIsEmpty(
            int invalidSequenceIndex,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextSequence = stream.NextSequence;
            var getNextSequenceFunc = new Func<int, EventStreamEventMetadata, EventStreamEntrySequence>(
                (currentIndex, eventMetadata) =>
                {
                    var currentValidSequence = nextSequence++;
                    Assert.NotEqual<uint>(eventMetadata.EntrySequence, currentValidSequence);
                    return currentIndex == invalidSequenceIndex ? eventMetadata.EntrySequence : currentValidSequence;
                });
            
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        getNextSequenceFunc(i, eventWithMetadata.EventMetadata),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEventsWithMetadata(eventsWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEventsWithMetadata_And_AllEventsWithMetadataHaveValidStreamIdAndValidSequence(
            EventStream stream,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            stream.AppendEventsWithMetadata(eventsWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEventsWithMetadata_And_AllEventsWithMetadataHaveValidStreamIdAndValidSequence_And_EventStreamIsEmpty(
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            stream.AppendEventsWithMetadata(eventsWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventWithMetadataSequenceIncreasedByOneAsNextSequence_When_GettingNextSequenceAfterAppendingEventsWithMetadata(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            stream.AppendEventsWithMetadata(eventsWithMetadata);
            
            Assert.Equal<uint>(eventsWithMetadata.Last().EventMetadata.EntrySequence + 1, stream.NextSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventWithMetadataSequenceIncreasedByOneAsNextSequence_When_GettingNextSequenceAfterAppendingEventsWithMetadata_And_EventStreamWasEmpty(
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            stream.AppendEventsWithMetadata(eventsWithMetadata);
            
            Assert.Equal<uint>(eventsWithMetadata.Last().EventMetadata.EntrySequence + 1, stream.NextSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventsWithMetadataAsEventsWithMetadataToAppend_When_GettingEventsWithMetadataToAppendAfterAppendingEventsWithMetadata(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            stream.AppendEventsWithMetadata(eventsWithMetadata);
            
            Assert.True(stream.EventsWithMetadataToAppend.SequenceEqual(eventsWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventsWithMetadataAsEventsWithMetadataToAppend_When_GettingEventsWithMetadataToAppendAfterAppendingEventsWithMetadata_EventStreamWasEmpty(
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            stream.AppendEventsWithMetadata(eventsWithMetadata);
            
            Assert.True(stream.EventsWithMetadataToAppend.SequenceEqual(eventsWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEmptyCollectionOfEventsWithMetadata(
            EventStreamId streamId)
        {
            var stream1 = new EventStream(streamId, new List<EventStreamEventWithMetadata>());
            var stream2 = new EventStream(streamId, new List<EventStreamEventWithMetadata>());
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEventsWithMetadata(
            EventStream eventStream)
        {
            var stream1 = new EventStream(eventStream.StreamId, eventStream.EventsWithMetadata);
            var stream2 = new EventStream(eventStream.StreamId, eventStream.EventsWithMetadata);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEventsWithMetadata_And_SameAppendedEventsWithMetadata(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream1 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream1.AppendEventsWithMetadata(eventsWithMetadata);

            var stream2 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream2.AppendEventsWithMetadata(eventsWithMetadata);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamIdAndEmptyCollectionOfEventsWithMetadata(
            EventStreamId streamId1,
            EventStreamId streamId2)
        {
            var stream1 = new EventStream(streamId1, new List<EventStreamEventWithMetadata>());
            var stream2 = new EventStream(streamId2, new List<EventStreamEventWithMetadata>());
            
            Assert.NotEqual(stream1.GetHashCode(), stream2.GetHashCode());
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamIdAndEventsWithMetadata(
            EventStream eventStream1,
            EventStream eventStream2)
        {
            var stream1 = new EventStream(eventStream1.StreamId, eventStream1.EventsWithMetadata);
            var stream2 = new EventStream(eventStream2.StreamId, eventStream2.EventsWithMetadata);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEventsWithMetadata_And_OneHasAppendedEventsWithMetadataWhileTheOtherDoesNot(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream1 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream1.AppendEventsWithMetadata(eventsWithMetadata);

            var stream2 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEventsWithMetadata_And_BothHaveDifferentNumberAppendedEventsWithMetadata(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream1 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream1.AppendEventsWithMetadata(eventsWithMetadata);

            var stream2 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream2.AppendEventsWithMetadata(eventsWithMetadata.Take(new Random().Next(0, eventsWithMetadata.Count)));
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEventsWithMetadata_And_BothHaveDifferentAppendedEventsWithMetadata(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata1,
            List<EventStreamEventWithMetadata> eventsWithMetadata2)
        {
            var validStreamId = stream.StreamId;
            var validNextSequence = stream.NextSequence;
            eventsWithMetadata1 = eventsWithMetadata1
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        validNextSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            validNextSequence = stream.NextSequence;
            eventsWithMetadata2 = eventsWithMetadata2
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        validNextSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();
            
            var stream1 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream1.AppendEventsWithMetadata(eventsWithMetadata1);

            var stream2 = new EventStream(stream.StreamId, stream.EventsWithMetadata);
            stream2.AppendEventsWithMetadata(eventsWithMetadata2);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStream eventStream)
        {
            var expectedValue =
                $"Event Stream ID: {eventStream.StreamId}, {eventStream.EventsWithMetadata}, Next Sequence: {eventStream.NextSequence}, {EventsWithMetadataToAppendString()}";

            string EventsWithMetadataToAppendString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata to append: ");
                foreach (var eventWithMetadata in eventStream.EventsWithMetadataToAppend)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }
            
            Assert.Equal(expectedValue, eventStream.ToString());
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToStringWithAppendedEntries(
            EventStream stream,
            List<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var validStreamId = stream.StreamId;
            var nextValidSequence = stream.NextSequence;
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        validStreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        nextValidSequence++,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)))
                .ToList();

            stream.AppendEventsWithMetadata(eventsWithMetadata);
            
            var expectedValue =
                $"Event Stream ID: {stream.StreamId}, {stream.EventsWithMetadata}, Next Sequence: {stream.NextSequence}, {EventsWithMetadataToAppendString()}";

            string EventsWithMetadataToAppendString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata to append: ");
                foreach (var eventWithMetadata  in stream.EventsWithMetadataToAppend)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }

            Assert.Equal(expectedValue, stream.ToString());
        }
    }
}