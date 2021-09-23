using System;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Configurations
{
    internal interface IRabbitMQPublishingChannelConfiguration
    {
        TimeSpan PublishingTimeout { get; }

        string ExchangeName { get; }
        
        string QueueName { get; }
        
        string RoutingKey { get; }
        
        string ExchangeType { get; }
    }
}