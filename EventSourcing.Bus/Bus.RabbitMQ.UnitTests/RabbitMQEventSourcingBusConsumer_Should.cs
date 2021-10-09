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
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                null,
                configuration));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CreatingWithNullConfiguration(
            IRabbitMQConnection connection)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                connection,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_CreatingWithNotNullParameters(
            IRabbitMQConnection connection,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            _ = new RabbitMQEventSourcingBusConsumer(
                connection,
                configuration);
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotCreateConsumingChannel_When_Created(
            Mock<IRabbitMQConnection> connectionMock,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            _ = new RabbitMQEventSourcingBusConsumer(
                connectionMock.Object,
                configuration);
            
            connectionMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task CreateConsumingChannel_When_StartedConsuming(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);

            connectionMock.Verify(connection => connection.CreateConsumingChannel(configuration), Times.Once);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task AddConsumerOnConsumingChannel_When_StartedConsuming(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            Mock<IRabbitMQConsumingChannel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            RabbitMQEventSourcingBusConsumer consumer,
            CancellationToken cancellationToken)
        {
            connectionMock
                .Setup(connection => connection.CreateConsumingChannel(It.IsAny<IRabbitMQConsumingChannelConfiguration>()))
                .Returns(consumingChannelMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, cancellationToken);
            
            consumingChannelMock.Verify(channel => channel.AddConsumer(consumingTaskFunc, cancellationToken), Times.Once);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_TryingToStartConsumingMultipleTimes(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);
            await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.StartConsuming(consumingTaskFunc, CancellationToken.None));
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
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            Mock<IRabbitMQConsumingChannel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            connectionMock
                .Setup(connection => connection.CreateConsumingChannel(It.IsAny<IRabbitMQConsumingChannelConfiguration>()))
                .Returns(consumingChannelMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);
            consumingChannelMock.Reset();
            
            await consumer.StopConsuming(CancellationToken.None);

            consumingChannelMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeConsumingChannel_When_StoppingConsuming_And_ConsumingWasStarted_When_ConsumingChannelImplementsIDisposable(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var consumingChannelMock = new Mock<IRabbitMQConsumingChannel>();
            var consumingChannelDisposableMock = consumingChannelMock.As<IDisposable>();
            connectionMock
                .Setup(connection => connection.CreateConsumingChannel(It.IsAny<IRabbitMQConsumingChannelConfiguration>()))
                .Returns(consumingChannelMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);
            consumingChannelMock.Reset();
            
            await consumer.StopConsuming(CancellationToken.None);

            consumingChannelDisposableMock.Verify(channel => channel.Dispose(), Times.Once);
            consumingChannelMock.VerifyNoOtherCalls();
        }
    }
}