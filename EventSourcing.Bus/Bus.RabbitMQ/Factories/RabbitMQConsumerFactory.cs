using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Factories
{
    internal class RabbitMQConsumerFactory : IRabbitMQConsumerFactory
    {
        public IBasicConsumer Create(
            IModel consumingChannel,
            Func<BasicDeliverEventArgs, CancellationToken, Task> handler,
            CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(consumingChannel);
            consumer.Received += async (_, args) =>
            {
                await handler(args, cancellationToken).ConfigureAwait(false);
                if (!cancellationToken.IsCancellationRequested)
                {
                    consumingChannel.BasicAck(args.DeliveryTag, false);
                }
            };

            return consumer;
        }
    }
}