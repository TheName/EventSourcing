using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConsumerFactory
    {
        Task<IRabbitMQConsumer<T>> CreateAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken);
    }
}