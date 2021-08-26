using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Factories
{
    internal interface IRabbitMQChannelFactory
    {
        IModel Create();
    }
}