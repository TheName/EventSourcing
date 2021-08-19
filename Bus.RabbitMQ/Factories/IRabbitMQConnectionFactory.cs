using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Factories
{
    internal interface IRabbitMQConnectionFactory
    {
        IConnection Create();
    }
}