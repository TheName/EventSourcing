using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Providers
{
    internal interface IRabbitMQChannelProvider
    {
        IModel PublishingChannel { get; }
    }
}