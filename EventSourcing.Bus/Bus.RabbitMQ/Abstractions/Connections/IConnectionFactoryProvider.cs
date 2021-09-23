using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Connections
{
    internal interface IConnectionFactoryProvider
    {
        IConnectionFactory Get();
    }
}