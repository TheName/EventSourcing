using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQProducingChannel
    {
        Task BindQueueAsync(IRabbitMQProducingQueueBindingConfiguration bindingConfiguration, CancellationToken cancellationToken);

        Task PublishAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken);
    }
}