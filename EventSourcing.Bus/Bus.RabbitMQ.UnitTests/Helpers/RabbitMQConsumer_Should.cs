using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Helpers;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Helpers
{
    public class RabbitMQConsumer_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullChannelProvider(IRabbitMQConsumerFactory consumerFactory)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConsumer(null, consumerFactory));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullConsumerFactory(IRabbitMQChannelProvider channelProvider)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConsumer(channelProvider, null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_With_NotNullParameters(IRabbitMQChannelProvider channelProvider, IRabbitMQConsumerFactory consumerFactory)
        {
            _ = new RabbitMQConsumer(channelProvider, consumerFactory);
        }

        [Theory]
        [AutoMoqData]
        internal void ConsumeWithCreatedConsumer(
            string queueName,
            Func<BasicDeliverEventArgs, CancellationToken, Task> handler,
            IBasicConsumer basicConsumer,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQChannelProvider> channelProviderMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQConsumer consumer)
        {
            channelProviderMock
                .Setup(provider => provider.ConsumingChannel)
                .Returns(consumingChannelMock.Object);

            consumerFactoryMock
                .Setup(factory => factory.Create(consumingChannelMock.Object, handler, It.IsAny<CancellationToken>()))
                .Returns(basicConsumer);
            
            consumer.Consume(queueName, handler);

            consumingChannelMock.Verify(
                model => model.BasicConsume(queueName, false, string.Empty, false, false, null, basicConsumer),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void NotCancelConsumingTag_When_StoppingConsuming_And_ConsumingWasNotStarted(
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQChannelProvider> channelProviderMock,
            RabbitMQConsumer consumer)
        {
            channelProviderMock
                .Setup(provider => provider.ConsumingChannel)
                .Returns(consumingChannelMock.Object);
            
            consumer.StopConsuming();
            
            consumingChannelMock.Verify(model => model.BasicCancel(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        internal void CancelConsumingTag_When_StoppingConsuming_And_ConsumingWasStarted(
            string queueName,
            Func<BasicDeliverEventArgs, CancellationToken, Task> handler,
            string consumingTag,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQChannelProvider> channelProviderMock,
            RabbitMQConsumer consumer)
        {
            channelProviderMock
                .Setup(provider => provider.ConsumingChannel)
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model =>
                    model.BasicConsume(
                        queueName,
                        false,
                        string.Empty,
                        false,
                        false,
                        null,
                        It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag);
            
            consumer.Consume(queueName, handler);
            consumer.StopConsuming();
            
            consumingChannelMock.Verify(model => model.BasicCancel(consumingTag), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void CancelAllConsumingTags_When_StoppingConsuming_And_ConsumingWasStartedMultipleTimes(
            string queueName,
            Func<BasicDeliverEventArgs, CancellationToken, Task> handler,
            List<string> consumingTags,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IRabbitMQChannelProvider> channelProviderMock,
            RabbitMQConsumer consumer)
        {
            channelProviderMock
                .Setup(provider => provider.ConsumingChannel)
                .Returns(consumingChannelMock.Object);

            var sequenceSetup = consumingChannelMock
                .SetupSequence(model =>
                    model.BasicConsume(
                        queueName,
                        false,
                        string.Empty,
                        false,
                        false,
                        null,
                        It.IsAny<IBasicConsumer>()));

            foreach (var consumingTag in consumingTags)
            {
                sequenceSetup.Returns(consumingTag);
                consumer.Consume(queueName, handler);
            }

            consumer.StopConsuming();

            consumingChannelMock.Verify(model => model.BasicCancel(It.IsAny<string>()), Times.Exactly(consumingTags.Count));
            foreach (var consumingTag in consumingTags)
            {
                consumingChannelMock.Verify(model => model.BasicCancel(consumingTag), Times.Once);
            }
        }
    }
}