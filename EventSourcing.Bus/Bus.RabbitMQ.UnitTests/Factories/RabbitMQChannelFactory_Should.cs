using System;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Factories;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Factories
{
    public class RabbitMQChannelFactory_Should
    {
        [Theory]
        [AutoMoqData]
        internal void ReturnProperlyCreatedChannel(
            string exchangeName,
            string exchangeType,
            string queueName,
            string routingKey,
            [Frozen] IRabbitMQPublishAcknowledgmentTracker rabbitMQPublishingChannel,
            [Frozen] Mock<IRabbitMQConnectionProvider> rabbitMQConnectionProviderMock,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQChannelFactory channelFactory)
        {
            var modelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            rabbitMQConnectionProviderMock
                .SetupGet(provider => provider.Connection)
                .Returns(connectionMock.Object);
            
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(modelMock.Object);

            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.ExchangeName)
                .Returns(exchangeName);

            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.ExchangeType)
                .Returns(exchangeType);

            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.QueueName)
                .Returns(queueName);

            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.RoutingKey)
                .Returns(routingKey);

            var channel = channelFactory.Create();

            Assert.Equal(modelMock.Object, channel);
            modelMock.Verify(model => model.ExchangeDeclare(exchangeName, exchangeType, true, false, null), Times.Once);
            modelMock.Verify(model => model.QueueDeclare(queueName, true, false, false, null), Times.Once);
            modelMock.Verify(model => model.QueueBind(queueName, exchangeName, routingKey, null), Times.Once);
            modelMock.Verify(model => model.ConfirmSelect(), Times.Once);
            modelMock.VerifyAdd(model => model.BasicAcks += rabbitMQPublishingChannel.AckEventHandler, Times.Once);
            modelMock.VerifyAdd(model => model.BasicNacks += rabbitMQPublishingChannel.NackEventHandler, Times.Once);
            modelMock.VerifyNoOtherCalls();
        }
    }
}