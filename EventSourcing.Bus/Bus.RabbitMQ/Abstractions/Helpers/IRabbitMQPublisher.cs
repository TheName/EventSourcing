using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Helpers
{
    internal interface IRabbitMQPublisher
    {
        Task PublishAsync<T>(
            T message,
            string exchangeName,
            string routingKey,
            IReadOnlyDictionary<string, string> headers,
            CancellationToken cancellationToken);
    }
}