using System;

namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal interface IRabbitMQProducingQueueBindingConfiguration
    {
        string ExchangeName { get; }
        
        string ExchangeType { get; }
        
        string QueueName { get; }
        
        string RoutingKey { get; }
        
        TimeSpan PublishingTimeout { get; }
    }
}