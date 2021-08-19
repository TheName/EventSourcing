using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal interface IRabbitMQChannelProvider
    {
        IModel PublishingChannel { get; }

        void Disconnect();
    }
}