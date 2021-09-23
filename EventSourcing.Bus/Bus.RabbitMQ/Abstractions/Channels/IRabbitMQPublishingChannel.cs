using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Channels
{
    internal interface IRabbitMQPublishingChannel
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    }
}