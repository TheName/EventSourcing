using System;
using System.Threading;

namespace EventSourcing.Bus.RabbitMQ.Extensions
{
    internal static class SemaphoreSlimExtensions
    {
        public static void TryRelease(this SemaphoreSlim semaphoreSlim)
        {
            try
            {
                semaphoreSlim.Release();
            }
            catch (ObjectDisposedException)
            {
                // do nothing
            }
        }
    }
}