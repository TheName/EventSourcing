using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class PublishingAcknowledgmentTracker : IDisposable
    {
        private readonly Dictionary<ulong, TaskCompletionSource<bool>> _deliveryTagsWithPublishingTaskCompletionSources
            = new Dictionary<ulong, TaskCompletionSource<bool>>();

        private readonly Channel<AckResult> _ackChannel = Channel.CreateUnbounded<AckResult>();

        private readonly CancellationTokenSource _publishingChannelCancellationTokenSource =
            new CancellationTokenSource();

        private readonly Task _acknowledgmentsReaderTask;
        private readonly Task _taskCompletionSourcesCleanerTask;

        private bool _disposed;

        public PublishingAcknowledgmentTracker()
        {
            _acknowledgmentsReaderTask = AcknowledgmentsReaderTask(_publishingChannelCancellationTokenSource.Token);
            _taskCompletionSourcesCleanerTask = TaskCompletionSourcesCleanerTask(_publishingChannelCancellationTokenSource.Token);
        }

        public Task<bool> RegisterDeliveryTag(ulong deliveryTag, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                _publishingChannelCancellationTokenSource.Token);
            cancellationTokenSource.CancelAfter(timeout);
            cancellationTokenSource.Token.Register(o => taskCompletionSource.TrySetCanceled(), null, false);

            _deliveryTagsWithPublishingTaskCompletionSources.Add(deliveryTag, taskCompletionSource);

            return taskCompletionSource.Task;
        }

        public void ChannelOnBasicAcks(object sender, BasicAckEventArgs ackEventArgs)
        {
            var result = new AckResult(ackEventArgs.DeliveryTag, ackEventArgs.Multiple, true);
            WriteToChannel(result);
        }

        public void ChannelOnBasicNacks(object sender, BasicNackEventArgs nackEventArgs)
        {
            var result = new AckResult(nackEventArgs.DeliveryTag, nackEventArgs.Multiple, false);
            WriteToChannel(result);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            
            _publishingChannelCancellationTokenSource?.Cancel();
            _publishingChannelCancellationTokenSource?.Dispose();
            _acknowledgmentsReaderTask?.ConfigureAwait(false).GetAwaiter().GetResult();
            _acknowledgmentsReaderTask?.Dispose();
            _taskCompletionSourcesCleanerTask?.ConfigureAwait(false).GetAwaiter().GetResult();
            _taskCompletionSourcesCleanerTask?.Dispose();
            foreach (var value in _deliveryTagsWithPublishingTaskCompletionSources.Values)
            {
                value?.TrySetCanceled();
            }

            _disposed = true;
        }

        private void WriteToChannel(AckResult ackResult)
        {
            _ackChannel.Writer.WriteAsync(ackResult, _publishingChannelCancellationTokenSource.Token)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private async Task AcknowledgmentsReaderTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                AckResult message;
                try
                {
                    message = await _ackChannel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                var deliveryTags = new List<ulong> {message.DeliveryTag};
                if (message.Multiple)
                {
                    deliveryTags = _deliveryTagsWithPublishingTaskCompletionSources.Keys
                        .Where(tag => tag <= message.DeliveryTag).ToList();
                }

                foreach (var tag in deliveryTags)
                {
                    if (_deliveryTagsWithPublishingTaskCompletionSources.TryGetValue(tag, out var taskCompletionSource))
                    {
                        taskCompletionSource.TrySetResult(message.WasSuccessful);
                    }

                    _deliveryTagsWithPublishingTaskCompletionSources.Remove(tag);
                }
            }
        }

        private async Task TaskCompletionSourcesCleanerTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                
                foreach (var pair in _deliveryTagsWithPublishingTaskCompletionSources.Where(pair => pair.Value.Task.IsCompleted))
                {
                    _deliveryTagsWithPublishingTaskCompletionSources.Remove(pair.Key);
                }
            }
        }

        private struct AckResult
        {
            public ulong DeliveryTag { get; }
            public bool Multiple { get; }
            public bool WasSuccessful { get; }

            public AckResult(ulong deliveryTag, bool multiple, bool wasSuccessful)
            {
                DeliveryTag = deliveryTag;
                Multiple = multiple;
                WasSuccessful = wasSuccessful;
            }
        }
    }
}