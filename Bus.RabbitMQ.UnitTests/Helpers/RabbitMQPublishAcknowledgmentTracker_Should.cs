using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Helpers;
using Moq;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Helpers
{
    public class RabbitMQPublishAcknowledgmentTracker_Should
    {
        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        internal async Task AcknowledgeExactDeliveryTag_RegardlessOfMultipleParameter(
            bool multiple,
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag, CancellationToken.None);

            tracker.AckEventHandler(null, new BasicAckEventArgs {DeliveryTag = deliveryTag, Multiple = multiple});

            var result = await taskCompletionSource.Task;

            Assert.True(result);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task AcknowledgeSmallerDeliveryTag_If_MultipleIsTrue(
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag - 1, CancellationToken.None);
            
            tracker.AckEventHandler(null, new BasicAckEventArgs {DeliveryTag = deliveryTag, Multiple = true});

            var result = await taskCompletionSource.Task;

            Assert.True(result);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task NotAcknowledgeSmallerDeliveryTag_If_MultipleIsFalse(
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag - 1, CancellationToken.None);
            
            tracker.AckEventHandler(null, new BasicAckEventArgs {DeliveryTag = deliveryTag, Multiple = false});

            await Assert.ThrowsAsync<TaskCanceledException>(() => taskCompletionSource.Task);
        }
        
        [Theory]
        [AutoMoqWithInlineData(true)]
        [AutoMoqWithInlineData(false)]
        internal async Task FailToAcknowledgeExactDeliveryTag_RegardlessOfMultipleParameter(
            bool multiple,
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag, CancellationToken.None);
            
            tracker.NackEventHandler(null, new BasicNackEventArgs {DeliveryTag = deliveryTag, Multiple = multiple});

            var result = await taskCompletionSource.Task;

            Assert.False(result);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task FailToAcknowledgeSmallerDeliveryTag_If_MultipleIsTrue(
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag - 1, CancellationToken.None);
            
            tracker.NackEventHandler(null, new BasicNackEventArgs {DeliveryTag = deliveryTag, Multiple = true});

            var result = await taskCompletionSource.Task;

            Assert.False(result);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task NotFailToAcknowledgeSmallerDeliveryTag_If_MultipleIsFalse(
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag - 1, CancellationToken.None);
            
            tracker.NackEventHandler(null, new BasicNackEventArgs {DeliveryTag = deliveryTag, Multiple = false});

            await Assert.ThrowsAsync<TaskCanceledException>(() => taskCompletionSource.Task);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task Throw_When_TimeoutOccurs(
            ulong deliveryTag,
            TimeSpan publishingTimeout,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQPublishAcknowledgmentTracker tracker)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.PublishingTimeout)
                .Returns(publishingTimeout + TimeSpan.FromSeconds(1));
            
            var taskCompletionSource = tracker.WaitForAcknowledgment(deliveryTag, CancellationToken.None);

            await Assert.ThrowsAsync<TaskCanceledException>(() => taskCompletionSource.Task);
        }
    }
}