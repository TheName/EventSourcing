using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQHandlingExceptionProducerFactory
    {
        Task<IRabbitMQProducer<T>> CreateAsync<T>(CancellationToken cancellationToken);
    }
}