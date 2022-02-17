using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bus.RabbitMQ.UnitTests.Helpers
{
    public static class CancellingTasksHelper
    {
        public static Func<CancellationToken, Task> CreateCancellingTask(Action cancellationRequestedAction)
        {
            var cancellingTask = new Func<CancellationToken, Task>(async token =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), token);
                    }
                    catch (TaskCanceledException)
                    {
                        // do nothing
                    }

                    if (token.IsCancellationRequested)
                    {
                        cancellationRequestedAction();
                        token.ThrowIfCancellationRequested();
                    }
                }
            });

            return cancellingTask;
        }
        
        public static Func<CancellationToken, Task<T>> CreateCancellingTaskWithReturnType<T>(Action cancellationRequestedAction)
        {
            var cancellingTask = new Func<CancellationToken, Task<T>>(async token =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), token);
                    }
                    catch (TaskCanceledException)
                    {
                        // do nothing
                    }

                    if (token.IsCancellationRequested)
                    {
                        cancellationRequestedAction();
                        token.ThrowIfCancellationRequested();
                    }
                }
            });

            return cancellingTask;
        }
        
        public static Func<T, CancellationToken, Task> CreateCancellingTask<T>(Action cancellationRequestedAction)
        {
            var cancellingTask = new Func<T, CancellationToken, Task>(async (_, token) =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), token);
                    }
                    catch (TaskCanceledException)
                    {
                        // do nothing
                    }

                    if (token.IsCancellationRequested)
                    {
                        cancellationRequestedAction();
                        token.ThrowIfCancellationRequested();
                    }
                }
            });

            return cancellingTask;
        }
        
        public static Func<TInput, CancellationToken, Task<TResult>> CreateCancellingTaskWithReturnType<TInput, TResult>(Action cancellationRequestedAction)
        {
            var cancellingTask = new Func<TInput, CancellationToken, Task<TResult>>(async (_, token) =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), token);
                    }
                    catch (TaskCanceledException)
                    {
                        // do nothing
                    }

                    if (token.IsCancellationRequested)
                    {
                        cancellationRequestedAction();
                        token.ThrowIfCancellationRequested();
                    }
                }
            });

            return cancellingTask;
        }
    }
}