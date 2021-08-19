using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class PublishingChannel : IPublishingChannel, IDisposable
    {
        private readonly Channel<ChannelMessage> _channel;
        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<bool>> _completionSources;
        private readonly CancellationTokenSource _readerTaskCancellationTokenSource;
        private readonly Task _readerTask;
        
        public PublishingChannel()
        {
            _channel = Channel.CreateUnbounded<ChannelMessage>();
            _completionSources = new ConcurrentDictionary<ulong, TaskCompletionSource<bool>>();
            _readerTaskCancellationTokenSource = new CancellationTokenSource();
            _readerTask = ChannelReader(_readerTaskCancellationTokenSource.Token);
        }
        
        public void Ack(ulong deliveryTag, bool multiple)
        {
            _channel.Writer.WriteAsync(new ChannelMessage(true, deliveryTag, multiple))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public void Nack(ulong deliveryTag, bool multiple)
        {
            _channel.Writer.WriteAsync(new ChannelMessage(false, deliveryTag, multiple))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public TaskCompletionSource<bool> WaitForAcknowledgment(ulong deliveryTag, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(9));
            cancellationTokenSource.Token.Register(o => taskCompletionSource.SetCanceled(), null, false);

            if (!_completionSources.TryAdd(deliveryTag, taskCompletionSource))
            {
                throw new Exception("Cannot register delivery tag.");
            }

            return taskCompletionSource;
        }

        private async Task ChannelReader(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var applicableDeliveryTags = new List<ulong> {message.DeliveryTag};
                if (message.Multiple)
                {
                    applicableDeliveryTags = _completionSources.Keys.Where(deliveryTag => deliveryTag <= message.DeliveryTag).ToList();
                }
                
                foreach (var deliveryTag in applicableDeliveryTags)
                {
                    SetResultAndRemove(deliveryTag, message.Ack);
                }
            }
        }

        private void SetResultAndRemove(ulong deliveryTag, bool result)
        {
            if (_completionSources.TryGetValue(deliveryTag, out var taskCompletionSource))
            {
                taskCompletionSource.TrySetResult(result);
            }

            _completionSources.TryRemove(deliveryTag, out taskCompletionSource);
        }
        
        private class ChannelMessage
        {
            public bool Ack { get; }
            public ulong DeliveryTag { get; }
            public bool Multiple { get; }

            public ChannelMessage(
                bool ack,
                ulong deliveryTag,
                bool multiple)
            {
                Ack = ack;
                DeliveryTag = deliveryTag;
                Multiple = multiple;
            }
        }

        public void Dispose()
        {
            _readerTaskCancellationTokenSource.Cancel();
            _readerTask.ConfigureAwait(false).GetAwaiter().GetResult();
            _readerTask?.Dispose();
            foreach (var taskCompletionSource in _completionSources.Values)
            {
                taskCompletionSource.SetCanceled();
            }
        }
    }
}