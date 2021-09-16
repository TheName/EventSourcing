using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Abstractions.Helpers;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.Abstractions.UnitTests.Helpers
{
    public class BaseEventStreamAggregateMethodInvoker_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingInvoke_And_PassingNullAsAggregate(EventStreamEventWithMetadata eventWithMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(null, eventWithMetadata));
        }
        
        [Fact]
        public void Throw_ArgumentNullException_When_CallingInvoke_And_PassingNullAsEventWithMetadata()
        {
            var aggregate = new TestAggregate();
            Assert.Throws<ArgumentNullException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, null));
        }

        [Theory]
        [AutoMoqData]
        public void DoNothing_When_CallingInvoke_And_AggregateDoesNotHaveHandlingMethodsForProvidedEventType_And_ShouldIgnoreMissingHandlersOnAggregate(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregate(true);

            BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_CallingInvoke_And_AggregateDoesNotHaveHandlingMethodsForProvidedEventType_And_ShouldNotIgnoreMissingHandlersOnAggregate(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregate();

            Assert.Throws<MissingMethodException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_CallingInvoke_And_AggregateHasStaticHandlingMethodForProvidedEventType(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithStaticHandlingMethod();

            Assert.Throws<MissingMethodException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_CallingInvoke_And_AggregateHasMultipleHandlingMethodsForProvidedEventType(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithMultipleHandlingMethodsForSameEventType();

            Assert.Throws<InvalidOperationException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_CallingInvoke_And_AggregateHasBothHandlingMethodsForProvidedEventTypeAndForProvidedEventTypeAndMetadata(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithHandlingMethodsForSameEventTypeAndEventTypeWithMetadata();

            Assert.Throws<InvalidOperationException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_CallingInvoke_And_AggregateHasMultipleHandlingMethodsForProvidedEventTypeWithMetadata(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithMultipleHandlingMethodsForSameEventTypeWithMetadata();

            Assert.Throws<InvalidOperationException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata));
        }

        [Theory]
        [AutoMoqData]
        public void InvokeMethod_When_CallingInvoke_And_AggregateHasHandlingMethodForProvidedEventType(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithHandlingMethodForEventType();
            Assert.Null(aggregate.HandledTestEvent);

            BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata);
            
            Assert.Equal(eventWithMetadata.Event, aggregate.HandledTestEvent);
        }

        [Theory]
        [AutoMoqData]
        public void InvokeMethod_When_CallingInvoke_And_AggregateHasHandlingMethodForProvidedEventTypeWithMetadata(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithHandlingMethodForEventTypeWithMetadata();
            Assert.Null(aggregate.HandledTestEvent);
            Assert.Null(aggregate.HandledEventMetadata);

            BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata);
            
            Assert.Equal(eventWithMetadata.Event, aggregate.HandledTestEvent);
            Assert.Equal(eventWithMetadata.EventMetadata, aggregate.HandledEventMetadata);
        }
        
        private class TestAggregate : BaseEventStreamAggregate
        {
            protected internal override bool ShouldIgnoreMissingHandlers { get; }

            public TestAggregate(bool shouldIgnoreMissingHandlers = false)
            {
                ShouldIgnoreMissingHandlers = shouldIgnoreMissingHandlers;
            }
        }

        private class TestAggregateWithStaticHandlingMethod : BaseEventStreamAggregate
        {
            private static void Handle(TestEvent testEvent)
            {
            } 
        }
        
        private class TestAggregateWithMultipleHandlingMethodsForSameEventType : BaseEventStreamAggregate
        {
            private void Handle(TestEvent testEvent)
            {
            }

            private void HandleEvent(TestEvent testEvent)
            {
            }
        }
        
        private class TestAggregateWithHandlingMethodsForSameEventTypeAndEventTypeWithMetadata : BaseEventStreamAggregate
        {
            private void Handle(TestEvent testEvent)
            {
            }

            private void Handle(TestEvent testEvent, EventStreamEventMetadata eventMetadata)
            {
            }
        }
        
        private class TestAggregateWithMultipleHandlingMethodsForSameEventTypeWithMetadata : BaseEventStreamAggregate
        {
            private void Handle(TestEvent testEvent, EventStreamEventMetadata eventMetadata)
            {
            }

            private void HandleEvent(TestEvent testEvent, EventStreamEventMetadata eventMetadata)
            {
            }
        }
        
        private class TestAggregateWithHandlingMethodForEventType : BaseEventStreamAggregate
        {
            public TestEvent HandledTestEvent { get; private set; }
            
            private void Handle(TestEvent testEvent)
            {
                HandledTestEvent = testEvent;
            }
        }
        
        private class TestAggregateWithHandlingMethodForEventTypeWithMetadata : BaseEventStreamAggregate
        {
            public TestEvent HandledTestEvent { get; private set; }
            public EventStreamEventMetadata HandledEventMetadata { get; private set; }
            
            private void Handle(TestEvent testEvent, EventStreamEventMetadata eventMetadata)
            {
                HandledTestEvent = testEvent;
                HandledEventMetadata = eventMetadata;
            }
        }
        
        private class TestEvent
        {
        }
    }
}