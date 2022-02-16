using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Configurations;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConsumingChannel
    {
        Task AcknowledgeAsync(ulong deliveryTag, CancellationToken cancellationToken);

        Task RejectAsync(ulong deliveryTag, CancellationToken cancellationToken);

        Task<bool> TryCancelConsumerAsync(string consumerTag, CancellationToken cancellationToken);

        Task BindQueueAsync(IRabbitMQConsumingQueueBindingConfiguration bindingConfiguration, CancellationToken cancellationToken);

        Task StartConsumingAsync(IBasicConsumer basicConsumer, CancellationToken cancellationToken);
    }
}