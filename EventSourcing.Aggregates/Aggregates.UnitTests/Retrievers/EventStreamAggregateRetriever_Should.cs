using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing;
using EventSourcing.Aggregates.Builders;
using EventSourcing.Aggregates.Retrievers;
using EventSourcing.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests.Retrievers
{
    public class EventStreamAggregateRetriever_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamRetriever(
            IEventStreamAggregateBuilder eventStreamAggregateBuilder)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamAggregateRetriever(null, eventStreamAggregateBuilder));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamAggregateBuilder(
            IEventStreamRetriever eventStreamRetriever)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamAggregateRetriever(eventStreamRetriever, null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNotNullParameters(
            IEventStreamRetriever eventStreamRetriever,
            IEventStreamAggregateBuilder eventStreamAggregateBuilder)
        {
            _ = new EventStreamAggregateRetriever(eventStreamRetriever, eventStreamAggregateBuilder);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RetrieveEventStream_When_RetrievingAggregate(
            Type aggregateType,
            EventStreamId streamId,
            EventStream eventStream,
            [Frozen] Mock<IEventStreamRetriever> eventStreamRetrieverMock,
            EventStreamAggregateRetriever aggregateRetriever)
        {
            eventStreamRetrieverMock
                .Setup(retriever => retriever.RetrieveAsync(streamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventStream)
                .Verifiable();

            _ = await aggregateRetriever.RetrieveAsync(aggregateType, streamId, CancellationToken.None);

            eventStreamRetrieverMock.Verify();
            eventStreamRetrieverMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnNull_When_RetrievingAggregate_And_EventStreamHasNoEvents(
            Type aggregateType,
            EventStreamId streamId,
            [Frozen] Mock<IEventStreamRetriever> eventStreamRetrieverMock,
            [Frozen] Mock<IEventStreamAggregateBuilder> eventStreamAggregateBuilderMock,
            EventStreamAggregateRetriever aggregateRetriever)
        {
            eventStreamRetrieverMock
                .Setup(retriever => retriever.RetrieveAsync(streamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventStream.NewEventStream(streamId))
                .Verifiable();

            var result = await aggregateRetriever.RetrieveAsync(aggregateType, streamId, CancellationToken.None);

            Assert.Null(result);
            eventStreamRetrieverMock.Verify();
            eventStreamRetrieverMock.VerifyNoOtherCalls();
            eventStreamAggregateBuilderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task BuildAggregateOfProvidedTypeAndRetrievedEventStream_When_RetrievingAggregate(
            Type aggregateType,
            EventStreamId streamId,
            EventStream eventStream,
            object builtAggregate,
            [Frozen] Mock<IEventStreamRetriever> eventStreamRetrieverMock,
            [Frozen] Mock<IEventStreamAggregateBuilder> eventStreamAggregateBuilderMock,
            EventStreamAggregateRetriever aggregateRetriever)
        {
            eventStreamRetrieverMock
                .Setup(retriever => retriever.RetrieveAsync(streamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventStream)
                .Verifiable();

            eventStreamAggregateBuilderMock
                .Setup(builder => builder.Build(aggregateType, eventStream))
                .Returns(builtAggregate)
                .Verifiable();

            var result = await aggregateRetriever.RetrieveAsync(aggregateType, streamId, CancellationToken.None);

            eventStreamRetrieverMock.Verify();
            eventStreamRetrieverMock.VerifyNoOtherCalls();
            eventStreamAggregateBuilderMock.Verify();
            eventStreamAggregateBuilderMock.VerifyNoOtherCalls();

            Assert.Equal(builtAggregate, result);
        }
    }
}
