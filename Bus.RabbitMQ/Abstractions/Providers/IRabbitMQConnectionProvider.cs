using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Providers
{
    internal interface IRabbitMQConnectionProvider 
    {
        IConnection Connection { get; }
    }
}