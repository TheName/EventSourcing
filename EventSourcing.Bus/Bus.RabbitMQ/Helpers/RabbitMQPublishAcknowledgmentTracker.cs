using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class RabbitMQPublishAcknowledgmentTracker : IRabbitMQPublishAcknowledgmentTracker, IDisposable
    {
        private readonly IRabbitMQConfigurationProvider _rabbitMQConfigurationProvider;
        private readonly Channel<ChannelMessage> _channel;
        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<bool>> _completionSources;
        private readonly CancellationTokenSource _readerTaskCancellationTokenSource;
        private readonly Task _readerTask;
        
        public RabbitMQPublishAcknowledgmentTracker(IRabbitMQConfigurationProvider rabbitMQConfigurationProvider)
        {
            _rabbitMQConfigurationProvider = rabbitMQConfigurationProvider;
            _channel = Channel.CreateUnbounded<ChannelMessage>();
            _completionSources = new ConcurrentDictionary<ulong, TaskCompletionSource<bool>>();
            _readerTaskCancellationTokenSource = new CancellationTokenSource();
            _readerTask = ChannelReader(_readerTaskCancellationTokenSource.Token);
        }

        public EventHandler<BasicAckEventArgs> AckEventHandler => (sender, args) => Ack(args.DeliveryTag, args.Multiple);

        public EventHandler<BasicNackEventArgs> NackEventHandler => (sender, args) => Nack(args.DeliveryTag, args.Multiple);
        
        private void Ack(ulong deliveryTag, bool multiple)
        {
            _channel.Writer.WriteAsync(new ChannelMessage(true, deliveryTag, multiple))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void Nack(ulong deliveryTag, bool multiple)
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
            cancellationTokenSource.CancelAfter(_rabbitMQConfigurationProvider.PublishingTimeout);
            cancellationTokenSource.Token.Register(o => taskCompletionSource.TrySetCanceled(), null, false);

            if (!_completionSources.TryAdd(deliveryTag, taskCompletionSource))
            {
                throw new Exception("Cannot register delivery tag.");
            }

            return taskCompletionSource;
        }

        public void Dispose()
        {
            _readerTaskCancellationTokenSource.Cancel();
            _readerTask.ConfigureAwait(false).GetAwaiter().GetResult();
            _readerTask?.Dispose();
            foreach (var taskCompletionSource in _completionSources.Values)
            {
                taskCompletionSource?.TrySetCanceled();
            }
        }

        private async Task ChannelReader(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ChannelMessage message;
                try
                {
                    message = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                
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
    }
}