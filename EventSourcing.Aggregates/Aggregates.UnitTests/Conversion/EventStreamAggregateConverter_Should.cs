using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Conversion;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests.Conversion
{
    public class EventStreamAggregateConverter_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_TryingToConvertObjectNotAssignableToIEventStreamAggregateToPublishableEventStream(
            object aggregate,
            EventStreamAggregateConverter converter)
        {
            Assert.Throws<NotSupportedException>(() => converter.ToPublishableEventStream(aggregate));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_TryingToConvertClassNotImplementingIEventStreamAggregateToPublishableEventStream(
            EventStreamAggregateConverter converter)
        {
            var aggregate = new NotImplementingIEventStreamAggregate();
            Assert.Throws<NotSupportedException>(() => converter.ToPublishableEventStream(aggregate));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnPublishableEventStream_When_TryingToConvertObjectImplementingIEventStreamAggregateToPublishableEventStream(
            PublishableEventStream publishableEventStream,
            Mock<IEventStreamAggregate> eventStreamAggregateMock,
            EventStreamAggregateConverter converter)
        {
            eventStreamAggregateMock
                .SetupGet(aggregate => aggregate.PublishableEventStream)
                .Returns(publishableEventStream);

            var result = converter.ToPublishableEventStream(eventStreamAggregateMock.Object);

            Assert.Equal(publishableEventStream, result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnPublishableEventStream_When_TryingToConvertClassImplementingIEventStreamAggregateToPublishableEventStream(
            PublishableEventStream publishableEventStream,
            EventStreamAggregateConverter converter)
        {
            var aggregate = new ImplementingIEventStreamAggregate(publishableEventStream);
            var result = converter.ToPublishableEventStream(aggregate);

            Assert.Equal(publishableEventStream, result);
        }
        
        private class NotImplementingIEventStreamAggregate
        {
        }
        
        private class ImplementingIEventStreamAggregate : IEventStreamAggregate 
        {
            public PublishableEventStream PublishableEventStream { get; }

            public ImplementingIEventStreamAggregate(PublishableEventStream publishableEventStream)
            {
                PublishableEventStream = publishableEventStream;
            }
            
            public void ReplayEventStream(EventStream eventStream)
            {
                throw new NotImplementedException();
            }
        }
    }
}