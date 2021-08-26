using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Providers
{
    internal interface IRabbitMQConnectionFactoryProvider
    {
        IConnectionFactory Get();
    }
}