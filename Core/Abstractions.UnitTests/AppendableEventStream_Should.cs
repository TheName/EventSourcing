using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions;
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
        public void Throw_ArgumentNullException_When_AppendingEventWithMetadata_And_PassingNullAsEvent(AppendableEventStream appendableEventStream)
        {
            Assert.Throws<ArgumentNullException>(() => appendableEventStream.AppendEventWithMetadata(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEventWithMetadata_And_PassingNotNullEvent(
            AppendableEventStream appendableEventStream,
            object @event)
        {
            appendableEventStream.AppendEventWithMetadata(@event);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventWithMetadata_When_AppendingEventWithMetadata(
            AppendableEventStream appendableEventStream,
            object @event)
        {
            var result = appendableEventStream.AppendEventWithMetadata(@event);
            
            Assert.Equal(appendableEventStream.EventsWithMetadataToAppend.Last(), result);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEvent_When_GettingEventsToAppend_After_AppendingEventWithMetadata(
            AppendableEventStream appendableEventStream,
            object @event,
            EventStreamEntryId entryId,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            appendableEventStream.AppendEventWithMetadata(@event, entryId, causationId, creationTime, correlationId);

            var eventToAppend = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(eventToAppend);
            Assert.Equal(@event, eventToAppend.Event);
            Assert.Equal(appendableEventStream.StreamId, eventToAppend.EventMetadata.StreamId);
            Assert.Equal(entryId, eventToAppend.EventMetadata.EntryId);
            Assert.Equal<EventStreamEntrySequence>(appendableEventStream.NextSequence - 1, eventToAppend.EventMetadata.EntrySequence);
            Assert.Equal(causationId, eventToAppend.EventMetadata.CausationId);
            Assert.Equal(creationTime, eventToAppend.EventMetadata.CreationTime);
            Assert.Equal(correlationId, eventToAppend.EventMetadata.CorrelationId);
        }

        [Theory]
        [AutoMoqData]
        public void UseRandomEntryId_When_Appending_And_PassingNullAsEntryId(
            AppendableEventStream appendableEventStream,
            object @event,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            appendableEventStream.AppendEventWithMetadata(@event, null, causationId, creationTime, correlationId);

            var eventToAppend = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(eventToAppend);
            Assert.NotNull(eventToAppend.EventMetadata.EntryId);
        }

        [Theory]
        [AutoMoqData]
        public void UseCurrentCausationId_When_Appending_And_PassingNullAsCausationId(
            AppendableEventStream appendableEventStream,
            object @event,
            EventStreamEntryId entryId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId)
        {
            appendableEventStream.AppendEventWithMetadata(@event, entryId, null, creationTime, correlationId);

            var eventToAppend = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(eventToAppend);
            Assert.Equal(EventStreamEntryCausationId.Current, eventToAppend.EventMetadata.CausationId);
        }

        [Theory]
        [AutoMoqData]
        public void UseNow_When_Appending_And_PassingNullAsCreationTime(
            AppendableEventStream appendableEventStream,
            object @event,
            EventStreamEntryId entryId,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCorrelationId correlationId)
        {
            appendableEventStream.AppendEventWithMetadata(@event, entryId, causationId, null, correlationId);

            var eventToAppend = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(eventToAppend);
            Assert.True((DateTime.UtcNow - eventToAppend.EventMetadata.CreationTime).TotalMilliseconds <= 1);
        }

        [Theory]
        [AutoMoqData]
        public void UseCurrentCorrelationId_When_Appending_And_PassingNullAsCorrelationId(
            AppendableEventStream appendableEventStream,
            object @event,
            EventStreamEntryId entryId,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime)
        {
            appendableEventStream.AppendEventWithMetadata(@event, entryId, causationId, creationTime, null);

            var eventToAppend = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(eventToAppend);
            Assert.Equal(EventStreamEntryCorrelationId.Current, eventToAppend.EventMetadata.CorrelationId);
        }

        [Theory]
        [AutoMoqData]
        public void IncreaseNextSequenceByOne_After_AppendingEventWithMetadata(
            AppendableEventStream appendableEventStream,
            object @event)
        {
            var previousNextSequence = appendableEventStream.NextSequence;
            
            appendableEventStream.AppendEventWithMetadata(@event);

            Assert.Equal<EventStreamEntrySequence>(previousNextSequence + 1, appendableEventStream.NextSequence);
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