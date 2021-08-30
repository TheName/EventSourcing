using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Factories;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Factories
{
    public class RabbitMQConsumerFactory_Should
    {
        [Theory]
        [AutoMoqData]
        internal void ReturnAsyncEventingBasicConsumer_When_Creating(
            IModel consumingChannel,
            Func<BasicDeliverEventArgs, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            RabbitMQConsumerFactory factory)
        {
            var result = factory.Create(consumingChannel, handler, cancellationToken);
            
            Assert.IsType<AsyncEventingBasicConsumer>(result);
        }

        [Theory]
        [AutoMoqData]
        internal async Task InvokeHandler_When_HandlingBasicDeliverOnCreatedConsumer(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body,
            IModel consumingChannel,
            CancellationToken cancellationToken,
            RabbitMQConsumerFactory factory)
        {
            var handlerInvoked = false;
            var handler = new Func<BasicDeliverEventArgs, CancellationToken, Task>((_, _) =>
            {
                handlerInvoked = true;
                return Task.CompletedTask;
            });

            var result = (AsyncDefaultBasicConsumer) factory.Create(consumingChannel, handler, cancellationToken);
            
            await result.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
            
            Assert.True(handlerInvoked);
        }

        [Theory]
        [AutoMoqData]
        internal async Task CallBasicAck_When_HandlingBasicDeliverOnCreatedConsumer_After_SuccessfulHandlerExecution(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body,
            Mock<IModel> consumingChannelMock,
            CancellationToken cancellationToken,
            RabbitMQConsumerFactory factory)
        {
            var handlerInvoked = false;
            var handler = new Func<BasicDeliverEventArgs, CancellationToken, Task>((_, _) =>
            {
                handlerInvoked = true;
                return Task.CompletedTask;
            });

            var result = (AsyncDefaultBasicConsumer) factory.Create(consumingChannelMock.Object, handler, cancellationToken);
            
            await result.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
            
            Assert.True(handlerInvoked);
            consumingChannelMock.Verify(model => model.BasicAck(deliveryTag, false), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task NotCallBasicAck_When_HandlingBasicDeliverOnCreatedConsumer_After_UnsuccessfulHandlerExecution(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body,
            Exception exception,
            Mock<IModel> consumingChannelMock,
            CancellationToken cancellationToken,
            RabbitMQConsumerFactory factory)
        {
            var handler = new Func<BasicDeliverEventArgs, CancellationToken, Task>((_, _) => Task.FromException(exception));

            var result = (AsyncDefaultBasicConsumer) factory.Create(consumingChannelMock.Object, handler, cancellationToken);
            
            await Assert.ThrowsAsync<Exception>(() => result.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body));
            
            consumingChannelMock.Verify(model => model.BasicAck(deliveryTag, false), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        internal async Task CancelHandlerExecution_When_AfterHandlingBasicDeliverOnCreatedConsumer_OriginalCancellationTokenIsCancelled(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body,
            Mock<IModel> consumingChannelMock,
            RabbitMQConsumerFactory factory)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var handler = new Func<BasicDeliverEventArgs, CancellationToken, Task>((_, token) => Task.Delay(-1, token));

            var result = (AsyncDefaultBasicConsumer) factory.Create(consumingChannelMock.Object, handler, cancellationTokenSource.Token);
            
            var executionTask = result.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
            
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() => executionTask);
        }
    }
}