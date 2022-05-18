using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStream_Should
    {
        [Fact]
        public void CreateNewEventStreamWithEmptyEntries_When_CallingNewEventStream()
        {
            var stream = EventStream.NewEventStream();
            Assert.NotNull(stream.StreamId);
            Assert.Empty(stream.EventsWithMetadata);
            Assert.Equal<uint>(0, stream.MaxSequence);
        }
        
        [Theory]
        [AutoMoqData]
        public void CreateNewEventStreamWithProvidedStreamIdAndEmptyEntries_When_CallingNewEventStream(EventStreamId streamId)
        {
            var stream = EventStream.NewEventStream(streamId);
            Assert.Equal(streamId, stream.StreamId);
            Assert.Empty(stream.EventsWithMetadata);
            Assert.Equal<uint>(0, stream.MaxSequence);
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
        public void NotThrow_When_CreatingWithNonNullStreamIdAndEmptyEventsWithMetadata(
            EventStreamId streamId)
        {
            _ = new EventStream(streamId, new List<EventStreamEventWithMetadata>());
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_CreatingWithDifferentStreamIdThanAssignedToEventsWithMetadata(
            EventStreamId streamId,
            EventStreamId streamIdAssignedToEventsWithMetadata,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select(eventWithMetadata => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamIdAssignedToEventsWithMetadata,
                        eventWithMetadata.EventMetadata.EntryId,
                        eventWithMetadata.EventMetadata.EntrySequence,
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));
            
            Assert.Throws<InvalidEventStreamIdException>(() => new EventStream(
                streamId, 
                eventsWithMetadata));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_Creating_And_AtLeastOneOfEventsWithMetadataHasInvalidStreamId_And_AllEventsWithMetadataHaveValidSequence(
            int invalidStreamIdIndex,
            EventStreamId streamId,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        i == invalidStreamIdIndex ? eventWithMetadata.EventMetadata.StreamId : streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            Assert.Throws<InvalidEventStreamIdException>(() => new EventStream(streamId, eventsWithMetadata));
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
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_Creating_And_AllEventsWithMetadataHaveValidStreamId_And_AtLeastOneOfEventsWithMetadataHasInvalidSequence(
            int invalidSequenceIndex,
            EventStreamId streamId,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            var getNextSequenceFunc = new Func<int, EventStreamEventMetadata, EventStreamEntrySequence>(
                (currentIndex, eventMetadata) =>
                {
                    var currentValidSequence = Convert.ToUInt32(currentIndex);
                    var currentInvalidSequence = eventMetadata.EntrySequence.Value;
                    if (currentInvalidSequence == currentValidSequence)
                    {
                        currentInvalidSequence = currentValidSequence + 1;
                    }
                    
                    return currentIndex == invalidSequenceIndex ? currentInvalidSequence : currentValidSequence;
                });
            
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        getNextSequenceFunc(i, eventWithMetadata.EventMetadata),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => new EventStream(streamId, eventsWithMetadata));
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
        public void Throw_InvalidEventStreamEntrySequenceException_When_CreatingWithSameStreamIdAsAssignedToEventsWithMetadata_And_EventsWithMetadataSequenceStartsWithZeroButAreNotIncreasedByOne(
            EventStreamId streamId,
            IEnumerable<EventStreamEventWithMetadata> eventsWithMetadata)
        {
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                         i == 0 ? 0 : Convert.ToUInt32(new Random().Next()),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));
            
            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => new EventStream(
                streamId,
                eventsWithMetadata));
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
        public void ReturnLastEventSequence_When_GettingMaxSequence(
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

            Assert.Equal<uint>(eventsWithMetadata.Last().EventMetadata.EntrySequence, stream.MaxSequence);
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
            AppendableEventStream eventStream1,
            AppendableEventStream eventStream2)
        {
            var stream1 = new EventStream(eventStream1.StreamId, eventStream1.EventsWithMetadata);
            var stream2 = new EventStream(eventStream2.StreamId, eventStream2.EventsWithMetadata);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStream eventStream)
        {
            var expectedValue =
                $"Event Stream ID: {eventStream.StreamId}, Max Sequence: {eventStream.MaxSequence}, {EventsWithMetadataString()}";

            string EventsWithMetadataString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata: ");
                foreach (var eventWithMetadata in eventStream.EventsWithMetadata)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }
            
            Assert.Equal(expectedValue, eventStream.ToString());
        }
    }
}