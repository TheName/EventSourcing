using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Helpers
{
    internal interface IRabbitMQConsumer
    {
        void Consume(string queueName, Func<BasicDeliverEventArgs, CancellationToken, Task> handler);
        
        void StopConsuming();
    }
}