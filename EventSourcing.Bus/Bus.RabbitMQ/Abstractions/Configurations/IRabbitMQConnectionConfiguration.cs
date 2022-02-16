using System;

namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal interface IRabbitMQConnectionConfiguration
    {
        string ClientName { get; }
        TimeSpan RequestedHeartbeat { get; }
        Uri Uri { get; }
        int ConsumerDispatchConcurrency { get; }
    }
}