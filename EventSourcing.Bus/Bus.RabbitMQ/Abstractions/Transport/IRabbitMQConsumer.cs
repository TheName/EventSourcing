using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConsumer<T>
    {
        Task StartConsumingAsync(CancellationToken cancellationToken);
        Task StopConsumingAsync(CancellationToken cancellationToken);
    }
}