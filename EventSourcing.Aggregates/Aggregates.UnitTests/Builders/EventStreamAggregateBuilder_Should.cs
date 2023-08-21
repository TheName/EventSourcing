using System;
using AutoFixture.Xunit2;
using EventSourcing.Aggregates;
using EventSourcing.Aggregates.Builders;
using EventSourcing.Aggregates.Factories;
using EventSourcing.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests.Builders
{
    public class EventStreamAggregateBuilder_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullAggregateFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamAggregateBuilder(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNotNullArguments(IEventStreamAggregateFactory aggregateFactory)
        {
            _ = new EventStreamAggregateBuilder(aggregateFactory);
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_FactoryReturnsNull(
            Type aggregateType,
            EventStream eventStream,
            [Frozen] Mock<IEventStreamAggregateFactory> eventSourcingAggregateFactoryMock,
            EventStreamAggregateBuilder aggregateBuilder)
        {
            eventSourcingAggregateFactoryMock
                .Setup(factory => factory.Create(aggregateType))
                .Returns(null);

            Assert.Throws<NotSupportedException>(() => aggregateBuilder.Build(aggregateType, eventStream));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_FactoryReturnsAnObjectThatDoesNotImplementIEventStreamAggregate(
            Type aggregateType,
            EventStream eventStream,
            object aggregateReturnedByFactory,
            [Frozen] Mock<IEventStreamAggregateFactory> eventSourcingAggregateFactoryMock,
            EventStreamAggregateBuilder aggregateBuilder)
        {
            eventSourcingAggregateFactoryMock
                .Setup(factory => factory.Create(aggregateType))
                .Returns(aggregateReturnedByFactory);

            Assert.Throws<NotSupportedException>(() => aggregateBuilder.Build(aggregateType, eventStream));
        }

        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_FactoryReturnsAnObjectThatDoesImplementIEventStreamAggregate(
            Type aggregateType,
            EventStream eventStream,
            IEventStreamAggregate aggregateReturnedByFactory,
            [Frozen] Mock<IEventStreamAggregateFactory> eventSourcingAggregateFactoryMock,
            EventStreamAggregateBuilder aggregateBuilder)
        {
            eventSourcingAggregateFactoryMock
                .Setup(factory => factory.Create(aggregateType))
                .Returns(aggregateReturnedByFactory);

            _ = aggregateBuilder.Build(aggregateType, eventStream);
        }

        [Theory]
        [AutoMoqData]
        internal void ReplayEventStreamOnAggregateReturnedByFactory_When_FactoryReturnsAnObjectThatDoesImplementIEventStreamAggregate(
            Type aggregateType,
            EventStream eventStream,
            Mock<IEventStreamAggregate> aggregateMockReturnedByFactory,
            [Frozen] Mock<IEventStreamAggregateFactory> eventSourcingAggregateFactoryMock,
            EventStreamAggregateBuilder aggregateBuilder)
        {
            eventSourcingAggregateFactoryMock
                .Setup(factory => factory.Create(aggregateType))
                .Returns(aggregateMockReturnedByFactory.Object);

            var result = aggregateBuilder.Build(aggregateType, eventStream);

            Assert.Equal(aggregateMockReturnedByFactory.Object, result);
            aggregateMockReturnedByFactory.Verify(aggregate => aggregate.ReplayEventStream(eventStream));
            aggregateMockReturnedByFactory.VerifyNoOtherCalls();
        }
    }
}
