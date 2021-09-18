using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions.Extensions;
using EventSourcing.Aggregates.Abstractions.Retrievers;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.Abstractions.UnitTests.Extensions
{
    public class EventStreamAggregateRetrieverExtensions_Should
    {
        [Theory]
        [AutoMoqData]
        public async Task Throw_When_CallingRetrieveAsync_And_PassingNullAsAggregateRetriever(EventStreamId streamId)
        {
            IEventStreamAggregateRetriever aggregateRetriever = null;
            await Assert.ThrowsAsync<ArgumentNullException>(() => aggregateRetriever.RetrieveAsync<TestAggregate>(streamId, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task CastReturnedObjectToProvidedType_When_CallingRetrieveAsync(
            Mock<IEventStreamAggregateRetriever> aggregateRetrieverMock,
            EventStreamId eventStreamId)
        {
            var aggregate = new TestAggregate();
            aggregateRetrieverMock
                .Setup(retriever => retriever.RetrieveAsync(typeof(TestAggregate), eventStreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aggregate);

            var aggregateRetriever = aggregateRetrieverMock.Object;

            var result = await aggregateRetriever.RetrieveAsync<TestAggregate>(eventStreamId, CancellationToken.None);
            
            Assert.Equal(aggregate, result);
        }
        
        private class TestAggregate
        {
        }
    }
}