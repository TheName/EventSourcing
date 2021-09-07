using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class AppendableEventStream_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStream()
        {
            Assert.Throws<ArgumentNullException>(() => new AppendableEventStream(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullParameters(EventStream eventStream)
        {
            _ = new AppendableEventStream(eventStream);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(EventStream eventStream)
        {
            var stream = new AppendableEventStream(eventStream);

            Assert.Equal(eventStream.StreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsWithMetadataProvidedDuringCreation_When_GettingEventsWithMetadata(EventStream eventStream)
        {
            var stream = new AppendableEventStream(eventStream);

            Assert.Same(eventStream.EventsWithMetadata, stream.EventsWithMetadata);
            Assert.Null(stream.EventsWithMetadata as List<EventStreamEventWithMetadata>);
            Assert.True(ReferenceEquals(eventStream.EventsWithMetadata, stream.EventsWithMetadata));
            Assert.Equal(eventStream.EventsWithMetadata, stream.EventsWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEmptyEventsWithMetadataToAppend_When_GettingEventsWithMetadataToAppend(EventStream eventStream)
        {
            var stream = new AppendableEventStream(eventStream);
            
            Assert.Empty(stream.EventsWithMetadataToAppend); 
        }

        [Theory]
        [AutoMoqData]
        public void ReturnMaxSequenceIncreasedByOne_When_GettingNextSequence(EventStream eventStream)
        {
            var stream = new AppendableEventStream(eventStream);
            
            Assert.Equal<uint>(eventStream.MaxSequence + 1, stream.NextSequence);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AppendingEventWithMetadata_And_PassingNull(AppendableEventStream appendableEventStream)
        {
            Assert.Throws<ArgumentNullException>(() => appendableEventStream.AppendEventWithMetadata(null));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEventWithMetadata_And_PassedEventStreamIdDoesNotMatch(
            AppendableEventStream appendableEventStream,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            Assert.NotEqual(appendableEventStream.StreamId, eventWithMetadata.EventMetadata.StreamId);
            Assert.Throws<InvalidEventStreamIdException>(() => appendableEventStream.AppendEventWithMetadata(eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEventWithMetadata_And_PassedEventStreamIdDoesMatch_And_PassedSequenceDoesNotMatchNextSequence(
            [Frozen(Matching.ExactType)] EventStreamId streamId,
            AppendableEventStream appendableEventStream,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            Assert.Equal(streamId, appendableEventStream.StreamId);
            Assert.Equal(streamId, eventWithMetadata.EventMetadata.StreamId);
            Assert.NotEqual(appendableEventStream.NextSequence, eventWithMetadata.EventMetadata.EntrySequence);
            
            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => appendableEventStream.AppendEventWithMetadata(eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEventWithMetadata_And_PassedEventStreamIdDoesMatch_And_PassedSequenceDoesMatchNextSequence(
            AppendableEventStream appendableEventStream,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(
                eventWithMetadata.Event,
                new EventStreamEventMetadata(
                    appendableEventStream.StreamId,
                    eventWithMetadata.EventMetadata.EntryId,
                    appendableEventStream.NextSequence,
                    eventWithMetadata.EventMetadata.CausationId,
                    eventWithMetadata.EventMetadata.CreationTime,
                    eventWithMetadata.EventMetadata.CorrelationId));

            appendableEventStream.AppendEventWithMetadata(eventWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEvent_When_GettingEventsToAppend_After_AppendingEventWithMetadata(
            AppendableEventStream appendableEventStream,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(
                eventWithMetadata.Event,
                new EventStreamEventMetadata(
                    appendableEventStream.StreamId,
                    eventWithMetadata.EventMetadata.EntryId,
                    appendableEventStream.NextSequence,
                    eventWithMetadata.EventMetadata.CausationId,
                    eventWithMetadata.EventMetadata.CreationTime,
                    eventWithMetadata.EventMetadata.CorrelationId));

            appendableEventStream.AppendEventWithMetadata(eventWithMetadata);

            var eventToAppend = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.Equal(eventWithMetadata, eventToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventsSequenceIncreasedByOne_When_GettingNextSequence_After_AppendingEventWithMetadata(
            AppendableEventStream appendableEventStream,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(
                eventWithMetadata.Event,
                new EventStreamEventMetadata(
                    appendableEventStream.StreamId,
                    eventWithMetadata.EventMetadata.EntryId,
                    appendableEventStream.NextSequence,
                    eventWithMetadata.EventMetadata.CausationId,
                    eventWithMetadata.EventMetadata.CreationTime,
                    eventWithMetadata.EventMetadata.CorrelationId));

            appendableEventStream.AppendEventWithMetadata(eventWithMetadata);

            Assert.Equal<uint>(eventWithMetadata.EventMetadata.EntrySequence + 1, appendableEventStream.NextSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameAppendableEventStream(AppendableEventStream appendableEventStream)
        {
            var stream1 = new PublishableEventStream(appendableEventStream);
            var stream2 = new PublishableEventStream(appendableEventStream);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentAppendableEventStream(
            AppendableEventStream appendableEventStream1,
            AppendableEventStream appendableEventStream2)
        {
            var stream1 = new PublishableEventStream(appendableEventStream1);
            var stream2 = new PublishableEventStream(appendableEventStream2);
            
            Assert.NotEqual(stream1.GetHashCode(), stream2.GetHashCode());
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(PublishableEventStream publishableEventStream)
        {
            var expectedValue =
                $"Event Stream ID: {publishableEventStream.StreamId}, {EventsWithMetadataString()}, {EventsWithMetadataToPublishString()}";

            string EventsWithMetadataString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata: ");
                foreach (var eventWithMetadata in publishableEventStream.EventsWithMetadata)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }
            
            string EventsWithMetadataToPublishString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata to publish: ");
                foreach (var eventWithMetadata in publishableEventStream.EventsWithMetadataToPublish)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }
            
            Assert.Equal(expectedValue, publishableEventStream.ToString());
        }
    }
}