using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class DisposingThreadLocal<T>
    {
        private readonly ThreadLocal<TerminationNotifyingDecorator> _threadLocal;
        private readonly Channel<T> _channel;
        private readonly Task _disposingThread;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ConcurrentQueue<T> _queue;

        public T Value
        {
            get
            {
                var result = _threadLocal.Value.Value;
                _queue.Enqueue(result);
                return result;
            }
        }

        public DisposingThreadLocal(Func<T> valueFunc)
        {
            _channel = Channel.CreateUnbounded<T>();
            _threadLocal = new ThreadLocal<TerminationNotifyingDecorator>(() => new TerminationNotifyingDecorator(valueFunc(), _channel.Writer));
            _cancellationTokenSource = new CancellationTokenSource();
            _disposingThread = DisposingThread(_cancellationTokenSource.Token);
            _queue = new ConcurrentQueue<T>();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _disposingThread.ConfigureAwait(false).GetAwaiter().GetResult();
            _disposingThread.Dispose();
            _threadLocal.Dispose();
            
            while (_queue.TryDequeue(out var dequeuedItem))
            {
                if (dequeuedItem is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private async Task DisposingThread(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                IDisposable disposable;
                try
                {
                    object dequeuedItem = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                    disposable = dequeuedItem as IDisposable;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                
                disposable?.Dispose();
            }
        }
        
        private class TerminationNotifyingDecorator
        {
            private readonly ChannelWriter<T> _channelWriter;
            public T Value { get; }

            public TerminationNotifyingDecorator(T value, ChannelWriter<T> channelWriter)
            {
                Value = value;
                _channelWriter = channelWriter;
            }

            ~TerminationNotifyingDecorator()
            {
                _channelWriter.TryWrite(Value);
            }
        }
    }
}