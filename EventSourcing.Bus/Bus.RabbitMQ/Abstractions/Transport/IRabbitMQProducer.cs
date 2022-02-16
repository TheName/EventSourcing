using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQProducer<in T>
    {
        Task PublishAsync(T message, CancellationToken cancellationToken);
    }
}