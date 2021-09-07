using System;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Abstractions.Factories;
using EventSourcing.Aggregates.Builders;
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
        public void NotThrow_When_CreatingWithNotNullArguments(IEventSourcingAggregateFactory aggregateFactory)
        {
            _ = new EventStreamAggregateBuilder(aggregateFactory);
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_NotSupportedException_When_FactoryReturnsNull(
            Type aggregateType,
            EventStream eventStream,
            [Frozen] Mock<IEventSourcingAggregateFactory> eventSourcingAggregateFactoryMock,
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
            [Frozen] Mock<IEventSourcingAggregateFactory> eventSourcingAggregateFactoryMock,
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
            [Frozen] Mock<IEventSourcingAggregateFactory> eventSourcingAggregateFactoryMock,
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
            [Frozen] Mock<IEventSourcingAggregateFactory> eventSourcingAggregateFactoryMock,
            EventStreamAggregateBuilder aggregateBuilder)
        {
            eventSourcingAggregateFactoryMock
                .Setup(factory => factory.Create(aggregateType))
                .Returns(aggregateMockReturnedByFactory.Object);

            var result = aggregateBuilder.Build(aggregateType, eventStream);
            
            Assert.Equal(aggregateMockReturnedByFactory.Object, result);
            aggregateMockReturnedByFactory.Verify(aggregate => aggregate.Replay(eventStream));
            aggregateMockReturnedByFactory.VerifyNoOtherCalls();
        }
    }
}