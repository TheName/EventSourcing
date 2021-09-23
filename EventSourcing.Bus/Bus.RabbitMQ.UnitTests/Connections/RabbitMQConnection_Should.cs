using System;
using AutoFixture.Xunit2;
using Bus.RabbitMQ.UnitTests.Extensions;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using EventSourcing.Bus.RabbitMQ.Channels;
using EventSourcing.Bus.RabbitMQ.Connections;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Connections
{
    public class RabbitMQConnection_Should
    {
        [Theory]
        [AutoMoqData]
        public void ThrowArgumentNullException_When_CreatingWithNullConnectionFactoryProvider(
            ISerializer serializer,
            ILoggerFactory loggerFactory)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConnection(null, serializer, loggerFactory));
        }
        
        [Theory]
        [AutoMoqData]
        internal void ThrowArgumentNullException_When_CreatingWithNullSerializer(
            IConnectionFactoryProvider connectionFactoryProvider,
            ILoggerFactory loggerFactory)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConnection(connectionFactoryProvider, null, loggerFactory));
        }
        
        [Theory]
        [AutoMoqData]
        internal void ThrowArgumentNullException_When_CreatingWithNullLoggerFactory(
            IConnectionFactoryProvider connectionFactoryProvider,
            ISerializer serializer)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConnection(connectionFactoryProvider, serializer, null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_CreatingWithNotNullParameters(
            IConnectionFactoryProvider connectionFactoryProvider,
            ISerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _ = new RabbitMQConnection(connectionFactoryProvider, serializer, loggerFactory);
        }

        [Theory]
        [AutoMoqData]
        internal void NotCreateRawConnection_When_Created(
            Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            ISerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _ = new RabbitMQConnection(connectionFactoryProviderMock.Object, serializer, loggerFactory);
            
            connectionFactoryProviderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void CreateRawConnection_When_CreatingPublishingChannel(
            IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);
            
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            
            connectionFactoryProviderMock.Verify(provider => provider.Get(), Times.Once);
            connectionFactoryMock.Verify(factory => factory.CreateConnection(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void CreateRawConnectionOnlyOnce_When_CreatingPublishingChannelMultipleTimes(
            IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);
            
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            
            connectionFactoryProviderMock.Verify(provider => provider.Get(), Times.Once);
            connectionFactoryMock.Verify(factory => factory.CreateConnection(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnNewInstanceOfRabbitMQPublishingChannel_When_CreatingPublishingChannel(
            IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration,
            IConnection rawConnection,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            [Frozen] ISerializer serializer,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);

            connectionFactoryMock
                .Setup(factory => factory.CreateConnection())
                .Returns(rawConnection);
            
            var result = connection.CreatePublishingChannel(publishingChannelConfiguration);

            var rabbitMQPublishingChannel = Assert.IsType<RabbitMQPublishingChannel>(result);
            Assert.NotNull(rabbitMQPublishingChannel);
            
            Assert.Equal(rawConnection, rabbitMQPublishingChannel.GetRawConnection());
            Assert.Equal(publishingChannelConfiguration, rabbitMQPublishingChannel.GetPublishingChannelConfiguration());
            Assert.Equal(serializer, rabbitMQPublishingChannel.GetSerializer());
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnNewInstanceOfRabbitMQPublishingChannelEveryTime_When_CreatingPublishingChannelMultipleTimes(
            IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration,
            RabbitMQConnection connection)
        {
            var result1 = connection.CreatePublishingChannel(publishingChannelConfiguration);
            var result2 = connection.CreatePublishingChannel(publishingChannelConfiguration);
            var result3 = connection.CreatePublishingChannel(publishingChannelConfiguration);

            Assert.NotEqual(result1, result2);
            Assert.NotEqual(result1, result3);
            Assert.NotEqual(result2, result3);
        }

        [Theory]
        [AutoMoqData]
        internal void CreateRawConnection_When_CreatingConsumingChannel(
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);
            
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            
            connectionFactoryProviderMock.Verify(provider => provider.Get(), Times.Once);
            connectionFactoryMock.Verify(factory => factory.CreateConnection(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void CreateRawConnectionOnlyOnce_When_CreatingConsumingChannelMultipleTimes(
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);
            
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            
            connectionFactoryProviderMock.Verify(provider => provider.Get(), Times.Once);
            connectionFactoryMock.Verify(factory => factory.CreateConnection(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnNewInstanceOfRabbitMQConsumingChannel_When_CreatingConsumingChannel(
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            IConnection rawConnection,
            ILogger logger,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            [Frozen] ISerializer serializer,
            [Frozen] Mock<ILoggerFactory> loggerFactoryMock,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);

            connectionFactoryMock
                .Setup(factory => factory.CreateConnection())
                .Returns(rawConnection);

            loggerFactoryMock
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(logger);
            
            var result = connection.CreateConsumingChannel(consumingChannelConfiguration);

            var rabbitMQConsumingChannel = Assert.IsType<RabbitMQConsumingChannel>(result);
            Assert.NotNull(rabbitMQConsumingChannel);
            
            Assert.Equal(rawConnection, rabbitMQConsumingChannel.GetRawConnection());
            Assert.Equal(consumingChannelConfiguration, rabbitMQConsumingChannel.GetConsumingChannelConfiguration());
            Assert.Equal(serializer, rabbitMQConsumingChannel.GetSerializer());
            Assert.IsType<Logger<RabbitMQConsumingChannel>>(rabbitMQConsumingChannel.GetLogger());
            Assert.Equal(logger, rabbitMQConsumingChannel.GetLogger().GetPrivateFieldValue<ILogger>("_logger"));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnNewInstanceOfRabbitMQConsumingChannelEveryTime_When_CreatingConsumingChannelMultipleTimes(
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            RabbitMQConnection connection)
        {
            var result1 = connection.CreateConsumingChannel(consumingChannelConfiguration);
            var result2 = connection.CreateConsumingChannel(consumingChannelConfiguration);
            var result3 = connection.CreateConsumingChannel(consumingChannelConfiguration);

            Assert.NotEqual(result1, result2);
            Assert.NotEqual(result1, result3);
            Assert.NotEqual(result2, result3);
        }

        [Theory]
        [AutoMoqData]
        internal void CreateRawConnectionOnlyOnce_When_CreatingPublishingChannelMultipleTimes_And_CreatingConsumingChannelMultipleTimes(
            IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration,
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            Mock<IConnectionFactory> connectionFactoryMock,
            [Frozen] Mock<IConnectionFactoryProvider> connectionFactoryProviderMock,
            RabbitMQConnection connection)
        {
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);
            
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            connection.CreatePublishingChannel(publishingChannelConfiguration);
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            connection.CreateConsumingChannel(consumingChannelConfiguration);
            
            connectionFactoryProviderMock.Verify(provider => provider.Get(), Times.Once);
            connectionFactoryMock.Verify(factory => factory.CreateConnection(), Times.Once);
        }
    }
}