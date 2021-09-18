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
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void Throw_ArgumentNullException_When_CallingInvoke_And_PassingNullAsAggregate(
            bool shouldIgnoreMissingHandlers,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            Assert.Throws<ArgumentNullException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(null, eventWithMetadata, shouldIgnoreMissingHandlers));
        }
        
        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void Throw_ArgumentNullException_When_CallingInvoke_And_PassingNullAsEventWithMetadata(bool shouldIgnoreMissingHandlers)
        {
            var aggregate = new TestAggregateWithoutHandlingMethods();
            Assert.Throws<ArgumentNullException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, null, shouldIgnoreMissingHandlers));
        }

        [Theory]
        [AutoMoqData]
        public void DoNothing_When_CallingInvoke_And_AggregateDoesNotHaveHandlingMethodsForProvidedEventType_And_ShouldIgnoreMissingHandlersOnAggregate(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithoutHandlingMethods();

            BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, true);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_CallingInvoke_And_AggregateDoesNotHaveHandlingMethodsForProvidedEventType_And_ShouldNotIgnoreMissingHandlersOnAggregate(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithoutHandlingMethods();

            Assert.Throws<MissingMethodException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, false));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_MissingMethodException_When_CallingInvoke_And_AggregateHasStaticHandlingMethodForProvidedEventType_And_ShouldNotIgnoreMissingHandlersOnAggregate(EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithStaticHandlingMethod();

            Assert.Throws<MissingMethodException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, false));
        }

        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void Throw_InvalidOperationException_When_CallingInvoke_And_AggregateHasMultipleHandlingMethodsForProvidedEventType(
            bool shouldIgnoreMissingHandlers,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithMultipleHandlingMethodsForSameEventType();

            Assert.Throws<InvalidOperationException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, shouldIgnoreMissingHandlers));
        }

        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void Throw_InvalidOperationException_When_CallingInvoke_And_AggregateHasBothHandlingMethodsForProvidedEventTypeAndForProvidedEventTypeAndMetadata(
            bool shouldIgnoreMissingHandlers,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithHandlingMethodsForSameEventTypeAndEventTypeWithMetadata();

            Assert.Throws<InvalidOperationException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, shouldIgnoreMissingHandlers));
        }

        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void Throw_InvalidOperationException_When_CallingInvoke_And_AggregateHasMultipleHandlingMethodsForProvidedEventTypeWithMetadata(
            bool shouldIgnoreMissingHandlers,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithMultipleHandlingMethodsForSameEventTypeWithMetadata();

            Assert.Throws<InvalidOperationException>(() => BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, shouldIgnoreMissingHandlers));
        }

        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void InvokeMethod_When_CallingInvoke_And_AggregateHasHandlingMethodForProvidedEventType(
            bool shouldIgnoreMissingHandlers,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithHandlingMethodForEventType();
            Assert.Null(aggregate.HandledTestEvent);

            BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, shouldIgnoreMissingHandlers);
            
            Assert.Equal(eventWithMetadata.Event, aggregate.HandledTestEvent);
        }

        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        public void InvokeMethod_When_CallingInvoke_And_AggregateHasHandlingMethodForProvidedEventTypeWithMetadata(
            bool shouldIgnoreMissingHandlers,
            EventStreamEventWithMetadata eventWithMetadata)
        {
            eventWithMetadata = new EventStreamEventWithMetadata(new TestEvent(), eventWithMetadata.EventMetadata);
            var aggregate = new TestAggregateWithHandlingMethodForEventTypeWithMetadata();
            Assert.Null(aggregate.HandledTestEvent);
            Assert.Null(aggregate.HandledEventMetadata);

            BaseEventStreamAggregateMethodInvoker.Invoke(aggregate, eventWithMetadata, shouldIgnoreMissingHandlers);
            
            Assert.Equal(eventWithMetadata.Event, aggregate.HandledTestEvent);
            Assert.Equal(eventWithMetadata.EventMetadata, aggregate.HandledEventMetadata);
        }
        
        private class TestAggregateWithoutHandlingMethods : BaseEventStreamAggregate
        {
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