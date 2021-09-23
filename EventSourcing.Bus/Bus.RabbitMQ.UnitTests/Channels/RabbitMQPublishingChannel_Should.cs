using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Channels;
using EventSourcing.Serialization.Abstractions;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Channels
{
    public class RabbitMQPublishingChannel_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConnectionIsNull(
            IRabbitMQPublishingChannelConfiguration configuration,
            ISerializer serializer)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQPublishingChannel(
                null,
                configuration,
                serializer));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConfigurationIsNull(
            IConnection connection,
            ISerializer serializer)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQPublishingChannel(
                connection,
                null,
                serializer));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_SerializerIsNull(
            IConnection connection,
            IRabbitMQPublishingChannelConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQPublishingChannel(
                connection,
                configuration,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IConnection connection,
            IRabbitMQPublishingChannelConfiguration configuration,
            ISerializer serializer)
        {
            _ = new RabbitMQPublishingChannel(
                connection,
                configuration,
                serializer);
        }

        [Theory]
        [AutoMoqData]
        internal void NotCreateChannel_When_Creating(
            Mock<IConnection> connectionMock,
            IRabbitMQPublishingChannelConfiguration configuration,
            ISerializer serializer)
        {
            _ = new RabbitMQPublishingChannel(
                connectionMock.Object,
                configuration,
                serializer);
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreateAndProperlyConfigureChannel_When_PublishingForTheFirstTime(
            object message,
            string exchangeName,
            string exchangeType,
            string queueName,
            string routingKey,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            configurationMock
                .SetupGet(configuration => configuration.ExchangeName)
                .Returns(exchangeName);

            configurationMock
                .SetupGet(configuration => configuration.ExchangeType)
                .Returns(exchangeType);

            configurationMock
                .SetupGet(configuration => configuration.QueueName)
                .Returns(queueName);

            configurationMock
                .SetupGet(configuration => configuration.RoutingKey)
                .Returns(routingKey);
            
            // without mocking acknowledgment we get a timeout.
            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            
            channelMock.Verify(channel => channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null), Times.Once);
            channelMock.Verify(channel => channel.QueueDeclare(queueName, true, false, false, null), Times.Once);
            channelMock.Verify(channel => channel.QueueBind(queueName, exchangeName, routingKey, null), Times.Once);
            channelMock.Verify(channel => channel.ConfirmSelect(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task NotCreateChannel_When_PublishingForTheNextTimesAfterFirstOne(
            object message,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            // Arrange; always return different publish sequence number
            channelMock
                .Setup(model => model.NextPublishSeqNo)
                .Returns(() => Convert.ToUInt64(new Random().Next()));

            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);
            
            // Arrange; publish first time and reset mock
            // without mocking acknowledgment we get a timeout.
            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            connectionMock.Reset();
            
            // Act; publish more times
            foreach (var i in Enumerable.Range(0, 3))
            {
                await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            }
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task BasicPublishSerializedMessageObjectToUtf8Bytes_With_CorrectProperties_When_Publishing(
            object message,
            byte[] serializedMessageObjectToUtf8Bytes,
            ulong deliveryTag,
            string exchangeName,
            string routingKey,
            Mock<IModel> channelMock,
            Mock<IBasicProperties> basicPropertiesMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            [Frozen] Mock<ISerializer> serializerMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            channelMock
                .Setup(model => model.NextPublishSeqNo)
                .Returns(deliveryTag);

            channelMock
                .Setup(model => model.CreateBasicProperties())
                .Returns(basicPropertiesMock.Object);

            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            configurationMock
                .SetupGet(configuration => configuration.ExchangeName)
                .Returns(exchangeName);

            configurationMock
                .SetupGet(configuration => configuration.RoutingKey)
                .Returns(routingKey);

            serializerMock
                .Setup(serializer => serializer.SerializeToUtf8Bytes(message))
                .Returns(serializedMessageObjectToUtf8Bytes);
            
            // without mocking acknowledgment we get a timeout.
            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            
            basicPropertiesMock.VerifySet(properties => properties.ContentType = "text/plain", Times.Once);
            basicPropertiesMock.VerifySet(properties => properties.DeliveryMode = 2, Times.Once);

            channelMock.Verify(channel => channel.BasicPublish(
                    exchangeName,
                    routingKey,
                    false,
                    basicPropertiesMock.Object,
                    serializedMessageObjectToUtf8Bytes),
                Times.Once);
        }

        [Theory]
        [AutoMoqWithInlineData("0:0:0:0.5")]
        [AutoMoqWithInlineData("0:0:0:1")]
        internal async Task ThrowTimeoutException_When_Publishing_And_AcknowledgmentIsNotSentWithinPublishingTimeout(
            string publishingTimeoutRaw,
            object message,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            var publishingTimeout = TimeSpan.Parse(publishingTimeoutRaw, CultureInfo.InvariantCulture);
            configurationMock
                .SetupGet(configuration => configuration.PublishingTimeout)
                .Returns(publishingTimeout);

            var stopwatch = Stopwatch.StartNew();
            
            await Assert.ThrowsAsync<TimeoutException>(() =>
                rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            
            stopwatch.Stop();
            
            Assert.True(stopwatch.Elapsed >= publishingTimeout);
        }

        [Theory]
        [AutoMoqWithInlineData("0:0:0:0.5")]
        [AutoMoqWithInlineData("0:0:0:1")]
        internal async Task NotThrowException_When_Publishing_And_AcknowledgmentIsReceivedBeforePublishingTimeoutExpires(
            string publishingTimeoutRaw,
            object message,
            ulong deliveryTag,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            var publishingTimeout = TimeSpan.Parse(publishingTimeoutRaw, CultureInfo.InvariantCulture);
            configurationMock
                .SetupGet(configuration => configuration.PublishingTimeout)
                .Returns(publishingTimeout);

            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            channelMock
                .SetupGet(channel => channel.NextPublishSeqNo)
                .Returns(deliveryTag);

            channelMock
                .Setup(channel => channel.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>((_, _, _, _, _) =>
                {
                    Task.Delay(publishingTimeout / 2).GetAwaiter().GetResult();
                    channelMock.Raise(model => model.BasicAcks += null,
                        new BasicAckEventArgs {DeliveryTag = deliveryTag, Multiple = false});
                });

            await rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None);
        }

        [Theory]
        [AutoMoqWithInlineData("0:0:0:0.5")]
        [AutoMoqWithInlineData("0:0:0:1")]
        internal async Task ThrowTimeoutException_When_Publishing_And_AcknowledgmentIsReceivedAfterPublishingTimeoutExpires(
            string publishingTimeoutRaw,
            object message,
            ulong deliveryTag,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            var publishingTimeout = TimeSpan.Parse(publishingTimeoutRaw, CultureInfo.InvariantCulture);
            configurationMock
                .SetupGet(configuration => configuration.PublishingTimeout)
                .Returns(publishingTimeout);

            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            channelMock
                .SetupGet(channel => channel.NextPublishSeqNo)
                .Returns(deliveryTag);

            channelMock
                .Setup(channel => channel.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>((_, _, _, _, _) =>
                {
                    Task.Delay(publishingTimeout).GetAwaiter().GetResult();
                    channelMock.Raise(model => model.BasicAcks += null,
                        new BasicAckEventArgs {DeliveryTag = deliveryTag, Multiple = false});
                });

            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData("0:0:0:0.5")]
        [AutoMoqWithInlineData("0:0:0:1")]
        internal async Task ThrowException_When_Publishing_And_NacknowledgmentIsReceivedBeforePublishingTimeoutExpires(
            string publishingTimeoutRaw,
            object message,
            ulong deliveryTag,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            var publishingTimeout = TimeSpan.Parse(publishingTimeoutRaw, CultureInfo.InvariantCulture);
            configurationMock
                .SetupGet(configuration => configuration.PublishingTimeout)
                .Returns(publishingTimeout);

            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            channelMock
                .SetupGet(channel => channel.NextPublishSeqNo)
                .Returns(deliveryTag);

            channelMock
                .Setup(channel => channel.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>((_, _, _, _, _) =>
                {
                    Task.Delay(publishingTimeout / 2).GetAwaiter().GetResult();
                    channelMock.Raise(model => model.BasicNacks += null,
                        new BasicNackEventArgs {DeliveryTag = deliveryTag, Multiple = false});
                });

            await Assert.ThrowsAsync<Exception>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData("0:0:0:0.5")]
        [AutoMoqWithInlineData("0:0:0:1")]
        internal async Task ThrowTimeoutException_When_Publishing_And_NacknowledgmentIsReceivedAfterPublishingTimeoutExpires(
            string publishingTimeoutRaw,
            object message,
            ulong deliveryTag,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQPublishingChannelConfiguration> configurationMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            var publishingTimeout = TimeSpan.Parse(publishingTimeoutRaw, CultureInfo.InvariantCulture);
            configurationMock
                .SetupGet(configuration => configuration.PublishingTimeout)
                .Returns(publishingTimeout);

            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            channelMock
                .SetupGet(channel => channel.NextPublishSeqNo)
                .Returns(deliveryTag);

            channelMock
                .Setup(channel => channel.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>((_, _, _, _, _) =>
                {
                    Task.Delay(publishingTimeout).GetAwaiter().GetResult();
                    channelMock.Raise(model => model.BasicNacks += null,
                        new BasicNackEventArgs {DeliveryTag = deliveryTag, Multiple = false});
                });

            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal void NotCreateChannel_When_Disposing_And_ChannelWasNotCreated(
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            rabbitMQPublishingChannel.Dispose();
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeChannel_When_Disposing_And_ChannelWasCreated(
            object message,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            // without mocking acknowledgment we get a timeout.
            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            
            rabbitMQPublishingChannel.Dispose();
            
            channelMock.Verify(model => model.Dispose(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeChannelOnce_When_DisposingMultipleTimes_And_ChannelWasCreated(
            object message,
            Mock<IModel> channelMock,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(channelMock.Object);

            // without mocking acknowledgment we get a timeout.
            await Assert.ThrowsAsync<TimeoutException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
            
            rabbitMQPublishingChannel.Dispose();
            rabbitMQPublishingChannel.Dispose();
            rabbitMQPublishingChannel.Dispose();
            
            channelMock.Verify(model => model.Dispose(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowObjectDisposedException_When_Publishing_AfterChannelWasDisposed(
            object message,
            RabbitMQPublishingChannel rabbitMQPublishingChannel)
        {
            rabbitMQPublishingChannel.Dispose();

            // without mocking acknowledgment we get a timeout.
            await Assert.ThrowsAsync<ObjectDisposedException>(() => rabbitMQPublishingChannel.PublishAsync(message, CancellationToken.None));
        }
    }
}