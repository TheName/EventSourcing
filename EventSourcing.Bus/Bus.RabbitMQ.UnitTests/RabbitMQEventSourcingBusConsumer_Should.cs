using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using EventSourcing.Bus.RabbitMQ.Configurations;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests
{
    public class RabbitMQEventSourcingBusConsumer_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CreatingWithNullConnection(
            EventSourcingRabbitMQChannelConfiguration configuration,
            IEventStreamEntryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                null,
                configuration,
                dispatcher));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CreatingWithNullConfiguration(
            IRabbitMQConnection connection,
            IEventStreamEntryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                connection,
                null,
                dispatcher));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CreatingWithNullDispatcher(
            IRabbitMQConnection connection,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                connection,
                configuration,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_CreatingWithNotNullParameters(
            IRabbitMQConnection connection,
            EventSourcingRabbitMQChannelConfiguration configuration,
            IEventStreamEntryDispatcher dispatcher)
        {
            _ = new RabbitMQEventSourcingBusConsumer(
                connection,
                configuration,
                dispatcher);
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotCreateConsumingChannel_When_Created(
            Mock<IRabbitMQConnection> connectionMock,
            EventSourcingRabbitMQChannelConfiguration configuration,
            IEventStreamEntryDispatcher dispatcher)
        {
            _ = new RabbitMQEventSourcingBusConsumer(
                connectionMock.Object,
                configuration,
                dispatcher);
            
            connectionMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task CreateConsumingChannel_When_StartedConsuming(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StartConsuming(CancellationToken.None);

            connectionMock.Verify(connection => connection.CreateConsumingChannel(configuration), Times.Once);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task AddConsumerOnConsumingChannel_When_StartedConsuming(
            Mock<IRabbitMQConsumingChannel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] IEventStreamEntryDispatcher dispatcher,
            RabbitMQEventSourcingBusConsumer consumer,
            CancellationToken cancellationToken)
        {
            connectionMock
                .Setup(connection => connection.CreateConsumingChannel(It.IsAny<IRabbitMQConsumingChannelConfiguration>()))
                .Returns(consumingChannelMock.Object);

            await consumer.StartConsuming(cancellationToken);
            
            consumingChannelMock.Verify(channel => channel.AddConsumer<EventStreamEntry>(dispatcher.DispatchAsync, cancellationToken), Times.Once);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_TryingToStartConsumingMultipleTimes(RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StartConsuming(CancellationToken.None);
            await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.StartConsuming(CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_StoppingConsuming_And_ConsumingWasNotStarted(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StopConsuming(CancellationToken.None);
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_StoppingConsuming_And_ConsumingWasStarted_When_ConsumingChannelDoesNotImplementIDisposable(
            Mock<IRabbitMQConsumingChannel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            connectionMock
                .Setup(connection => connection.CreateConsumingChannel(It.IsAny<IRabbitMQConsumingChannelConfiguration>()))
                .Returns(consumingChannelMock.Object);

            await consumer.StartConsuming(CancellationToken.None);
            consumingChannelMock.Reset();
            
            await consumer.StopConsuming(CancellationToken.None);

            consumingChannelMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeConsumingChannel_When_StoppingConsuming_And_ConsumingWasStarted_When_ConsumingChannelImplementsIDisposable(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var consumingChannelMock = new Mock<IRabbitMQConsumingChannel>();
            var consumingChannelDisposableMock = consumingChannelMock.As<IDisposable>();
            connectionMock
                .Setup(connection => connection.CreateConsumingChannel(It.IsAny<IRabbitMQConsumingChannelConfiguration>()))
                .Returns(consumingChannelMock.Object);

            await consumer.StartConsuming(CancellationToken.None);
            consumingChannelMock.Reset();
            
            await consumer.StopConsuming(CancellationToken.None);

            consumingChannelDisposableMock.Verify(channel => channel.Dispose(), Times.Once);
            consumingChannelMock.VerifyNoOtherCalls();
        }
    }
}