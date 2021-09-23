using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Helpers;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Helpers
{
    public class PublishingAcknowledgmentTracker_Should
    {
        [Theory]
        [AutoMoqData]
        internal void DisposeGracefully(PublishingAcknowledgmentTracker tracker)
        {
            tracker.Dispose();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnTaskThatThrowsTaskCancelledExceptionAfterProvidedTimeout_When_RegisteringDeliveryTag(
            PublishingAcknowledgmentTracker tracker,
            ulong deliveryTag,
            TimeSpan timeout)
        {
            timeout += TimeSpan.FromSeconds(1);
            var stopwatch = Stopwatch.StartNew();
            var task = tracker.RegisterDeliveryTag(deliveryTag, timeout, CancellationToken.None);

            await Assert.ThrowsAsync<TaskCanceledException>(() => task);
            stopwatch.Stop();
            
            Assert.True(stopwatch.Elapsed >= timeout, $"{stopwatch.Elapsed} >= {timeout}");
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnTaskThatThrowsTaskCancelledException_When_RegisteringDeliveryTag_And_CancellationTokenIsAlreadyCancelled(
            PublishingAcknowledgmentTracker tracker,
            ulong deliveryTag,
            TimeSpan timeout)
        {
            timeout += TimeSpan.FromSeconds(1);
            var stopwatch = Stopwatch.StartNew();
            var task = tracker.RegisterDeliveryTag(deliveryTag, timeout, new CancellationToken(true));

            await Assert.ThrowsAsync<TaskCanceledException>(() => task);
            stopwatch.Stop();
            
            Assert.True(stopwatch.Elapsed < timeout, $"{stopwatch.Elapsed} < {timeout}");
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnTaskThatReturnsFalse_When_RegisteringDeliveryTag_And_CallingChannelOnBasicNacks_And_DeliveryTagWasRegistered(
            PublishingAcknowledgmentTracker tracker,
            ulong deliveryTag,
            TimeSpan timeout)
        {
            timeout += TimeSpan.FromSeconds(1);
            var task = tracker.RegisterDeliveryTag(deliveryTag, timeout, CancellationToken.None);

            tracker.ChannelOnBasicNacks(null, new BasicNackEventArgs {DeliveryTag = deliveryTag});

            var result = await task;
            
            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnTaskThatReturnsTrue_When_RegisteringDeliveryTag_And_CallingChannelOnBasicAcks_And_DeliveryTagWasRegistered(
            PublishingAcknowledgmentTracker tracker,
            ulong deliveryTag,
            TimeSpan timeout)
        {
            timeout += TimeSpan.FromSeconds(1);
            var task = tracker.RegisterDeliveryTag(deliveryTag, timeout, CancellationToken.None);

            tracker.ChannelOnBasicAcks(null, new BasicAckEventArgs {DeliveryTag = deliveryTag});

            var result = await task;
            
            Assert.True(result);
        }
    }
}