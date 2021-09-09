using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests
{
    public class BaseEventStreamAggregate_Should
    {
        [Theory]
        [AutoMoqData]
        public void InvokeMethodHandlingSampleEventOnTestAggregate(
            EventStreamId streamId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEvent>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    Guid.NewGuid(),
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregate();
            testAggregate.Replay(stream);
            
            Assert.Equal(sampleEvent.Id, testAggregate.HandledSampleEventId);
        }
        
        [Theory]
        [AutoMoqData]
        public void InvokeMethodHandlingSampleEventWithoutIdAndEventMetadataOnTestAggregate(
            EventStreamId streamId,
            EventStreamEntryId entryId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEventWithoutId>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    entryId,
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregate();
            testAggregate.Replay(stream);
            
            Assert.Equal<Guid>(entryId, testAggregate.HandledSampleEventWithoutIdEntryId);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_When_HandlingSampleEventOnTestAggregateWithoutHandlers(
            EventStreamId streamId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEvent>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    Guid.NewGuid(),
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithoutHandlers();
            Assert.Throws<MissingMethodException>(() => testAggregate.Replay(stream));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_When_HandlingSampleEventWithoutIdAndEventMetadataOnTestAggregateWithoutHandlers(
            EventStreamId streamId,
            EventStreamEntryId entryId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEventWithoutId>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    entryId,
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithoutHandlers();
            Assert.Throws<MissingMethodException>(() => testAggregate.Replay(stream));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_HandlingSampleEventOnTestAggregateWithoutHandlersAndIgnoringMissingHandlers(
            EventStreamId streamId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEvent>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    Guid.NewGuid(),
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();
            testAggregate.Replay(stream);
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_HandlingSampleEventWithoutIdAndEventMetadataOnTestAggregateWithoutHandlersAndIgnoringMissingHandlers(
            EventStreamId streamId,
            EventStreamEntryId entryId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEventWithoutId>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    entryId,
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();
            testAggregate.Replay(stream);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_When_HandlingSampleEventOnTestAggregateWithBothTypesOfHandlers(
            EventStreamId streamId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEvent>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    Guid.NewGuid(),
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithBothTypesOfHandlers();
            Assert.Throws<InvalidOperationException>(() => testAggregate.Replay(stream));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_When_HandlingSampleEventOnTestAggregateWithMoreHandlersOfSameTypeWithDifferentName(
            EventStreamId streamId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEvent>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    Guid.NewGuid(),
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithMoreHandlersOfSameTypeWithDifferentName();
            Assert.Throws<InvalidOperationException>(() => testAggregate.Replay(stream));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_When_HandlingSampleEventOnTestAggregateWithMoreHandlersOfSameTypeAndEventMetadataWithDifferentName(
            EventStreamId streamId,
            IFixture fixture)
        {
            var sampleEvent = fixture.Create<SampleEvent>();
            var stream = new EventStream(streamId, new List<EventStreamEventWithMetadata>
            {
                new EventStreamEventWithMetadata(sampleEvent, new EventStreamEventMetadata(
                    streamId,
                    Guid.NewGuid(),
                    0,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid()))
            });

            var testAggregate = new TestAggregateWithMoreHandlersOfSameTypeAndEventMetadataWithDifferentName();
            Assert.Throws<InvalidOperationException>(() => testAggregate.Replay(stream));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventWithMetadataInEventsToPublish_When_GettingPublishableEventStream_After_AppendingAnEventWithMetadata(
            object @event,
            EventStreamEventMetadata eventMetadata)
        {
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();
            eventMetadata = new EventStreamEventMetadata(
                aggregate.PublishableEventStream.StreamId,
                eventMetadata.EntryId,
                0,
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);

            aggregate.AppendEventWithMetadata(@event, eventMetadata);

            var publishableEventStream = aggregate.PublishableEventStream;
            var singleEventWithMetadataToPublish = Assert.Single(publishableEventStream.EventsWithMetadataToPublish);
            Assert.NotNull(singleEventWithMetadataToPublish);
            Assert.Equal(@event, singleEventWithMetadataToPublish.Event);
            Assert.Equal(eventMetadata, singleEventWithMetadataToPublish.EventMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEventWithMetadataInEventsToPublish_When_GettingPublishableEventStream_After_AppendingAnEventWithCausationIdAndCorrelationId(
            object @event,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCorrelationId correlationId)
        {
            var now = DateTime.UtcNow;
            var aggregate = new TestAggregateWithoutHandlersAndIgnoringMissingHandlers();

            aggregate.AppendEventWithCausationIdAndCorrelationId(@event, causationId, correlationId);

            var publishableEventStream = aggregate.PublishableEventStream;
            var singleEventWithMetadataToPublish = Assert.Single(publishableEventStream.EventsWithMetadataToPublish);
            Assert.NotNull(singleEventWithMetadataToPublish);
            Assert.Equal(@event, singleEventWithMetadataToPublish.Event);
            Assert.Equal(aggregate.PublishableEventStream.StreamId, singleEventWithMetadataToPublish.EventMetadata.StreamId);
            Assert.NotNull(singleEventWithMetadataToPublish.EventMetadata.EntryId);
            Assert.NotEqual<Guid>(Guid.Empty, singleEventWithMetadataToPublish.EventMetadata.EntryId);
            Assert.Equal<uint>(0, singleEventWithMetadataToPublish.EventMetadata.EntrySequence);
            Assert.Equal(causationId, singleEventWithMetadataToPublish.EventMetadata.CausationId);
            Assert.True(singleEventWithMetadataToPublish.EventMetadata.CreationTime - now < TimeSpan.FromMilliseconds(10));
            Assert.Equal(correlationId, singleEventWithMetadataToPublish.EventMetadata.CorrelationId);
        }

        private class TestAggregateWithoutHandlers : BaseEventStreamAggregate
        {
        }

        private class TestAggregateWithoutHandlersAndIgnoringMissingHandlers : BaseEventStreamAggregate
        {
            protected override bool ShouldIgnoreMissingHandlers => true;

            public void AppendEventWithMetadata(object @event, EventStreamEventMetadata eventMetadata)
            {
                Append(@event, eventMetadata);
            }

            public void AppendEventWithCausationIdAndCorrelationId(
                object @event,
                EventStreamEntryCausationId causationId,
                EventStreamEntryCorrelationId correlationId)
            {
                Append(@event, causationId, correlationId);
            }
        }

        private class TestAggregateWithBothTypesOfHandlers : BaseEventStreamAggregate
        {
            private void FirstType(SampleEvent sampleEvent)
            {
            }

            private void SecondType(SampleEvent sampleEvent, EventStreamEventMetadata eventMetadata)
            {
            }
        }

        private class TestAggregateWithMoreHandlersOfSameTypeWithDifferentName : BaseEventStreamAggregate
        {
            private void FirstName(SampleEvent sampleEvent)
            {
            }

            private void SecondName(SampleEvent sampleEvent)
            {
            }
        }
        
        private class TestAggregateWithMoreHandlersOfSameTypeAndEventMetadataWithDifferentName : BaseEventStreamAggregate
        {
            private void FirstName(SampleEvent sampleEvent, EventStreamEventMetadata eventMetadata)
            {
            }

            private void SecondName(SampleEvent sampleEvent, EventStreamEventMetadata eventMetadata)
            {
            }
        }

        private class TestAggregate : BaseEventStreamAggregate
        {
            public Guid HandledSampleEventId { get; private set; }
            public Guid HandledSampleEventWithoutIdEntryId { get; private set; }
            
            private void Handle(SampleEvent @event)
            {
                HandledSampleEventId = @event.Id;
            }

            private void Handle(SampleEventWithoutId eventWithoutId, EventStreamEventMetadata eventMetadata)
            {
                HandledSampleEventWithoutIdEntryId = eventMetadata.EntryId;
            }
        }
        
        private class SampleEvent
        {
            public Guid Id { get; }

            public SampleEvent(Guid id)
            {
                Id = id;
            }
        }
        
        private class SampleEventWithoutId
        {
        }
    }
}