using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Bus.RabbitMQ.Extensions
{
    internal static class CancellationTokenSourceExtensions
    {
        public static void TryCancel(this CancellationTokenSource cancellationTokenSource, ILogger logger)
        {
            if (cancellationTokenSource == null)
            {
                return;
            }

            try
            {
                cancellationTokenSource.Cancel();
            }
            catch (Exception e)
            {
                logger?.LogInformation(
                    e,
                    "Exception was thrown when tried to cancel CancellationTokenSource; ignoring the exception");
            }
        }
    }
}