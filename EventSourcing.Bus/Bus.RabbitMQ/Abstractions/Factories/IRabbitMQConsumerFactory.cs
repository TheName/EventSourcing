using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Factories
{
    internal interface IRabbitMQConsumerFactory
    {
        IBasicConsumer Create(
            IModel consumingChannel, 
            Func<BasicDeliverEventArgs, CancellationToken, Task> handler,
            CancellationToken cancellationToken);
    }
}