using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConnectionConfigurationProvider
    {
        IRabbitMQConnectionConfiguration ConsumerConnectionConfiguration { get; }
        IRabbitMQConnectionConfiguration ProducerConnectionConfiguration { get; }
        IRabbitMQConnectionConfiguration HandlingExceptionProducerConnectionConfiguration { get; }
    }
}