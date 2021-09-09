using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Extensions.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Extensions.Abstractions.UnitTests
{
    public class AppendableEventStreamExtensions_Should
    {
        [Theory]
        [AutoMoqData]
        public void AppendEventWithProvidedMetadataAndCorrectStreamIdAndSequence_When_AppendingEventWithMetadata(
            object @event,
            EventStreamEntryId entryId,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCreationTime creationTime,
            EventStreamEntryCorrelationId correlationId,
            AppendableEventStream appendableEventStream)
        {
            var expectedSequence = appendableEventStream.NextSequence;
            appendableEventStream.AppendEventWithMetadata(
                @event,
                entryId,
                causationId,
                creationTime,
                correlationId);

            var singleAppendedEventWithMetadata = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(singleAppendedEventWithMetadata);
            
            Assert.Equal(@event, singleAppendedEventWithMetadata.Event);
            Assert.Equal(appendableEventStream.StreamId, singleAppendedEventWithMetadata.EventMetadata.StreamId);
            Assert.Equal(entryId, singleAppendedEventWithMetadata.EventMetadata.EntryId);
            Assert.Equal(expectedSequence, singleAppendedEventWithMetadata.EventMetadata.EntrySequence);
            Assert.Equal(causationId, singleAppendedEventWithMetadata.EventMetadata.CausationId);
            Assert.Equal(creationTime, singleAppendedEventWithMetadata.EventMetadata.CreationTime);
            Assert.Equal(correlationId, singleAppendedEventWithMetadata.EventMetadata.CorrelationId);
        }
        
        [Theory]
        [AutoMoqData]
        public void AppendEventWithGeneratedMetadata_When_AppendingEventWithMetadataWithoutProvidingOptionalParameters(
            object @event,
            AppendableEventStream appendableEventStream)
        {
            var expectedSequence = appendableEventStream.NextSequence;
            var now = DateTime.UtcNow;
            appendableEventStream.AppendEventWithMetadata(@event);

            var singleAppendedEventWithMetadata = Assert.Single(appendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(singleAppendedEventWithMetadata);
            
            Assert.Equal(@event, singleAppendedEventWithMetadata.Event);
            Assert.Equal(appendableEventStream.StreamId, singleAppendedEventWithMetadata.EventMetadata.StreamId);
            Assert.NotNull(singleAppendedEventWithMetadata.EventMetadata.EntryId);
            Assert.NotEqual<Guid>(Guid.Empty, singleAppendedEventWithMetadata.EventMetadata.EntryId);
            Assert.Equal(expectedSequence, singleAppendedEventWithMetadata.EventMetadata.EntrySequence);
            Assert.NotNull(singleAppendedEventWithMetadata.EventMetadata.CausationId);
            Assert.NotEqual<Guid>(Guid.Empty, singleAppendedEventWithMetadata.EventMetadata.CausationId);
            Assert.True(singleAppendedEventWithMetadata.EventMetadata.CreationTime - now < TimeSpan.FromMilliseconds(10));
            Assert.NotNull(singleAppendedEventWithMetadata.EventMetadata.CorrelationId);
            Assert.NotEqual<Guid>(Guid.Empty, singleAppendedEventWithMetadata.EventMetadata.CorrelationId);
        }
    }
}