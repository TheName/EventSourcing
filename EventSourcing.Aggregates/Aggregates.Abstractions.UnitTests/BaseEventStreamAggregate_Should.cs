using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.Abstractions.UnitTests
{
    public class BaseEventStreamAggregate_Should
    {
        [Fact]
        public void CreateEmptyEventStream_When_CreatingNewAggregate()
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();
            
            Assert.Empty(aggregate.AppendableEventStream.EventsWithMetadata);
            Assert.Empty(aggregate.AppendableEventStream.EventsWithMetadataToAppend);
            Assert.Empty(aggregate.PublishableEventStream.EventsWithMetadata);
            Assert.Empty(aggregate.PublishableEventStream.EventsWithMetadataToPublish);
        }

        [Theory]
        [AutoMoqData]
        public void UseProvidedEventStreamToCreateAppendableAndPublishableEventStream_When_ReplayingEventStream(EventStream eventStream)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();

            aggregate.Replay(eventStream);
            
            Assert.Equal(eventStream.StreamId, aggregate.AppendableEventStream.StreamId);
            Assert.Equal(eventStream.EventsWithMetadata, aggregate.AppendableEventStream.EventsWithMetadata);
            Assert.Equal<EventStreamEntrySequence>(eventStream.MaxSequence + 1, aggregate.AppendableEventStream.NextSequence);
            Assert.Empty(aggregate.AppendableEventStream.EventsWithMetadataToAppend);
            Assert.Equal(eventStream.StreamId, aggregate.PublishableEventStream.StreamId);
            Assert.Equal(eventStream.EventsWithMetadata, aggregate.PublishableEventStream.EventsWithMetadata);
            Assert.Empty(aggregate.PublishableEventStream.EventsWithMetadataToPublish);
        }

        [Theory]
        [AutoMoqData]
        public void InvokeEventHandlerForEveryEventFromEventStream_When_ReplayingEventStream(EventStream eventStream)
        {
            var aggregate = new TestAggregateWithEventHandlers();

            aggregate.Replay(eventStream);
            
            Assert.True(eventStream.EventsWithMetadata.Select(eventWithMetadata => eventWithMetadata.Event).SequenceEqual(aggregate.HandledEvents));
        }

        [Theory]
        [AutoMoqData]
        public void InvokeEventHandlerForEveryEventWithMetadataFromEventStream_When_ReplayingEventStream(EventStream eventStream)
        {
            var aggregate = new TestAggregateWithEventWithEventMetadataHandlers();

            aggregate.Replay(eventStream);
            
            Assert.Equal(eventStream.EventsWithMetadata.Count, aggregate.HandledEventsWithMetadata.Count);
            for (var i = 0; i < eventStream.EventsWithMetadata.Count; i++)
            {
                Assert.Equal(eventStream.EventsWithMetadata[i].Event, aggregate.HandledEventsWithMetadata[i].Item1);
                Assert.Equal(eventStream.EventsWithMetadata[i].EventMetadata, aggregate.HandledEventsWithMetadata[i].Item2);
            }
        }

        [Theory]
        [AutoMoqData]
        public void DoNothing_When_ReplayingEventStream_And_AggregateHasMissingHandlers_And_ShouldIgnoreMissingHandlers(EventStream eventStream)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();

            aggregate.Replay(eventStream);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_ReplayingEventStream_And_AggregateHasMissingHandlers_And_ShouldNotIgnoreMissingHandlers(EventStream eventStream)
        {
            var aggregate = new TestAggregateWithoutHandlersAndNotIgnoringMissingHandlers();

            Assert.Throws<MissingMethodException>(() => aggregate.Replay(eventStream));
        }

        [Theory]
        [AutoMoqData]
        public void AppendProvidedEventWithDefaultMetadata_When_AppendingEvent(object eventToAppend, EventStream eventStream)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();
            aggregate.Replay(eventStream);

            aggregate.Append(eventToAppend);

            var singleEventToAppend = Assert.Single(aggregate.AppendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(singleEventToAppend);
            Assert.Equal(eventToAppend, singleEventToAppend.Event);
            Assert.Equal(eventStream.StreamId, singleEventToAppend.EventMetadata.StreamId);
            Assert.NotNull(singleEventToAppend.EventMetadata.EntryId);
            Assert.Equal<EventStreamEntrySequence>(eventStream.MaxSequence + 1, singleEventToAppend.EventMetadata.EntrySequence);
            Assert.Equal(EventStreamEntryCausationId.Current, singleEventToAppend.EventMetadata.CausationId);
            Assert.True(DateTime.UtcNow - singleEventToAppend.EventMetadata.CreationTime < TimeSpan.FromMilliseconds(10));
            Assert.Equal(EventStreamEntryCorrelationId.Current, singleEventToAppend.EventMetadata.CorrelationId);
        }

        [Theory]
        [AutoMoqData]
        public void ContainAppendedEventAsEventToPublish_When_AppendingEvent(object eventToAppend, EventStream eventStream)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();
            aggregate.Replay(eventStream);

            aggregate.Append(eventToAppend);

            var singleEventToAppend = Assert.Single(aggregate.AppendableEventStream.EventsWithMetadataToAppend);
            var singleEventToPublish = Assert.Single(aggregate.PublishableEventStream.EventsWithMetadataToPublish);
            Assert.Equal(singleEventToAppend, singleEventToPublish);
        }

        [Theory]
        [AutoMoqData]
        public void InvokeEventHandlerForAppendedEvent_When_AppendingEvent(object eventToAppend)
        {
            var aggregate = new TestAggregateWithEventHandlers();

            aggregate.Append(eventToAppend);

            var singleHandledEvent = Assert.Single(aggregate.HandledEvents);
            Assert.NotNull(singleHandledEvent);
            Assert.Equal(eventToAppend, singleHandledEvent);
        }

        [Theory]
        [AutoMoqData]
        public void InvokeEventWithMetadataHandlerForAppendedEvent_When_AppendingEvent(object eventToAppend)
        {
            var aggregate = new TestAggregateWithEventWithEventMetadataHandlers();

            aggregate.Append(eventToAppend);

            var singleEventWithMetadataToAppend = Assert.Single(aggregate.AppendableEventStream.EventsWithMetadataToAppend);
            Assert.NotNull(singleEventWithMetadataToAppend);
            var (handledEvent, handledEventMetadata) = Assert.Single(aggregate.HandledEventsWithMetadata);
            Assert.Equal(singleEventWithMetadataToAppend.Event, handledEvent);
            Assert.Equal(singleEventWithMetadataToAppend.EventMetadata, handledEventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void DoNothing_When_AppendingEvent_And_AggregateHasMissingHandlers_And_ShouldIgnoreMissingHandlers(object eventToAppend)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();

            aggregate.Append(eventToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_AppendingEvent_And_AggregateHasMissingHandlers_And_ShouldNotIgnoreMissingHandlers(object eventToAppend)
        {
            var aggregate = new TestAggregateWithoutHandlersAndNotIgnoringMissingHandlers();

            Assert.Throws<MissingMethodException>(() => aggregate.Append(eventToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void InvokeEventHandlerForAppendedEvent_When_ReplayingEventWithMetadata(EventStreamEventWithMetadata eventWithMetadata)
        {
            var aggregate = new TestAggregateWithEventHandlers();

            aggregate.ReplayEvent(eventWithMetadata);

            var singleHandledEvent = Assert.Single(aggregate.HandledEvents);
            Assert.NotNull(singleHandledEvent);
            Assert.Equal(eventWithMetadata.Event, singleHandledEvent);
        }

        [Theory]
        [AutoMoqData]
        public void InvokeEventWithMetadataHandlerForAppendedEvent_When_ReplayingEventWithMetadata(EventStreamEventWithMetadata eventWithMetadata)
        {
            var aggregate = new TestAggregateWithEventWithEventMetadataHandlers();

            aggregate.ReplayEvent(eventWithMetadata);

            var (handledEvent, handledEventMetadata) = Assert.Single(aggregate.HandledEventsWithMetadata);
            Assert.Equal(eventWithMetadata.Event, handledEvent);
            Assert.Equal(eventWithMetadata.EventMetadata, handledEventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void DoNothing_When_ReplayingEventWithMetadata_And_AggregateHasMissingHandlers_And_ShouldIgnoreMissingHandlers(EventStreamEventWithMetadata eventWithMetadata)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();

            aggregate.ReplayEvent(eventWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_ReplayingEventWithMetadata_And_AggregateHasMissingHandlers_And_ShouldNotIgnoreMissingHandlers(EventStreamEventWithMetadata eventWithMetadata)
        {
            var aggregate = new TestAggregateWithoutHandlersAndNotIgnoringMissingHandlers();

            Assert.Throws<MissingMethodException>(() => aggregate.ReplayEvent(eventWithMetadata));
        }
        
        private class TestAggregateWithoutHandlersAndIgnoringMissingHandlers : BaseEventStreamAggregate
        {
            protected internal override bool ShouldIgnoreMissingHandlers => true;
        }
        
        private class TestAggregateWithoutHandlersAndNotIgnoringMissingHandlers : BaseEventStreamAggregate
        {
            protected internal override bool ShouldIgnoreMissingHandlers => false;
        }
        
        private class TestAggregateWithEventHandlers : BaseEventStreamAggregate
        {
            public List<object> HandledEvents { get; set; } = new List<object>();
            
            private void Handle(object @event)
            {
                HandledEvents.Add(@event);
            }
        }
        
        private class TestAggregateWithEventWithEventMetadataHandlers : BaseEventStreamAggregate
        {
            public List<(object, EventStreamEventMetadata)> HandledEventsWithMetadata { get; set; } =
                new List<(object, EventStreamEventMetadata)>();
            
            private void Handle(object @event, EventStreamEventMetadata eventMetadata)
            {
                HandledEventsWithMetadata.Add((@event, eventMetadata));
            }
        }
    }
}