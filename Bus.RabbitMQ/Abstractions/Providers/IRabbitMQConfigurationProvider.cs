using System;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Providers
{
    internal interface IRabbitMQConfigurationProvider
    {
        string ExchangeName { get; }
        string ExchangeType { get; }
        string QueueName { get; }
        string RoutingKey { get; }
        string ClientProvidedName { get; }
        string ConnectionString { get; }
        TimeSpan PublishingTimeout { get; }
    }
}