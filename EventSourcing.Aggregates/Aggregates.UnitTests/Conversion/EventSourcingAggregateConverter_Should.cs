using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Conversion;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests.Conversion
{
    public class EventSourcingAggregateConverter_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_TryingToConvertObjectNotAssignableToIEventStreamAggregateToPublishableEventStream(
            object aggregate,
            EventSourcingAggregateConverter converter)
        {
            Assert.Throws<NotSupportedException>(() => converter.ToPublishableEventStream(aggregate));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_TryingToConvertClassNotImplementingIEventStreamAggregateToPublishableEventStream(
            EventSourcingAggregateConverter converter)
        {
            var aggregate = new NotImplementingIEventStreamAggregate();
            Assert.Throws<NotSupportedException>(() => converter.ToPublishableEventStream(aggregate));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnPublishableEventStream_When_TryingToConvertObjectImplementingIEventStreamAggregateToPublishableEventStream(
            PublishableEventStream publishableEventStream,
            Mock<IEventStreamAggregate> eventStreamAggregateMock,
            EventSourcingAggregateConverter converter)
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
            EventSourcingAggregateConverter converter)
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