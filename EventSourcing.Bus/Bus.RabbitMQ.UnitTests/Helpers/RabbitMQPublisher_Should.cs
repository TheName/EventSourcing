using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Helpers;
using EventSourcing.Serialization.Abstractions;
using Moq;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Helpers
{
    public class RabbitMQPublisher_Should
    {
        [Theory]
        [AutoMoqData]
        internal async Task PublishSerializedMessageWithProperlySetPropertiesAndHeadersAndAwaitAcknowledgment(
            object message,
            string exchangeName,
            string routingKey,
            ulong deliveryTag,
            byte[] serializedMessage,
            Dictionary<string, string> headers,
            Mock<IBasicProperties> basicPropertiesMock,
            Mock<IModel> publishingChannelMock,
            [Frozen] Mock<IRabbitMQChannelProvider> channelProviderMock,
            [Frozen] Mock<ISerializer> serializerMock,
            [Frozen] Mock<IRabbitMQPublishAcknowledgmentTracker> publishAcknowledgmentTrackerMock,
            RabbitMQPublisher publisher)
        {
            publishingChannelMock
                .Setup(model => model.CreateBasicProperties())
                .Returns(basicPropertiesMock.Object);

            publishingChannelMock
                .SetupGet(model => model.NextPublishSeqNo)
                .Returns(deliveryTag);

            channelProviderMock
                .SetupGet(provider => provider.PublishingChannel)
                .Returns(publishingChannelMock.Object);

            var taskCompletionSource = new TaskCompletionSource<bool>();
            taskCompletionSource.SetResult(true);

            publishAcknowledgmentTrackerMock
                .Setup(tracker => tracker.WaitForAcknowledgment(deliveryTag, CancellationToken.None))
                .Returns(taskCompletionSource);

            serializerMock
                .Setup(serializer => serializer.SerializeToUtf8Bytes(message))
                .Returns(serializedMessage);

            await publisher.PublishAsync(
                message,
                exchangeName,
                routingKey,
                headers,
                CancellationToken.None);

            var assertProperties = new Func<IBasicProperties, bool>(properties =>
            {
                Assert.Equal("text/plain", properties.ContentType);
                Assert.Equal(2, properties.DeliveryMode);
                Assert.Equal(headers.Count, properties.Headers.Count);
                Assert.All(headers.Keys, key => Assert.Equal(headers[key], properties.Headers[key]));

                return true;
            });

            var assertMemory = new Func<ReadOnlyMemory<byte>, bool>(memory => serializedMessage.SequenceEqual(memory.ToArray()));

            publishingChannelMock
                .Verify(model => model.BasicPublish(
                        exchangeName,
                        routingKey,
                        false,
                        It.Is<IBasicProperties>(properties => assertProperties(properties)),
                        It.Is<ReadOnlyMemory<byte>>(memory => assertMemory(memory))),
                    Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_When_AcknowledgmentTaskIsCancelledDueToTimeout(
            object message,
            string exchangeName,
            string routingKey,
            Dictionary<string, string> headers,
            [Frozen] Mock<IRabbitMQPublishAcknowledgmentTracker> publishAcknowledgmentTrackerMock,
            RabbitMQPublisher publisher)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            publishAcknowledgmentTrackerMock
                .Setup(tracker => tracker.WaitForAcknowledgment(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .Returns(taskCompletionSource);
            
            taskCompletionSource.SetCanceled();

            await Assert.ThrowsAsync<TimeoutException>(() => publisher.PublishAsync(
                message,
                exchangeName,
                routingKey,
                headers,
                CancellationToken.None));
        } 

        [Theory]
        [AutoMoqData]
        internal async Task Throw_When_AcknowledgmentTaskReturnsFalse(
            object message,
            string exchangeName,
            string routingKey,
            Dictionary<string, string> headers,
            [Frozen] Mock<IRabbitMQPublishAcknowledgmentTracker> publishAcknowledgmentTrackerMock,
            RabbitMQPublisher publisher)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            publishAcknowledgmentTrackerMock
                .Setup(tracker => tracker.WaitForAcknowledgment(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .Returns(taskCompletionSource);
            
            taskCompletionSource.SetResult(false);

            await Assert.ThrowsAsync<Exception>(() => publisher.PublishAsync(
                message,
                exchangeName,
                routingKey,
                headers,
                CancellationToken.None));
        } 
    }
}