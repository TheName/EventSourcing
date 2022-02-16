using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQChannel
    {
        Task ExecuteActionInThreadSafeMannerAsync(Action<IModel> action, CancellationToken cancellationToken);
    }
}