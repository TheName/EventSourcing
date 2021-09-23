using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Channels
{
    internal interface IRabbitMQConsumingChannel
    {
        void AddConsumer<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken);
    }
}