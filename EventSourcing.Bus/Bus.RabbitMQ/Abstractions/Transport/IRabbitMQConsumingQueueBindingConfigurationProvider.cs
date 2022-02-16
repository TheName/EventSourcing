using System;
using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConsumingQueueBindingConfigurationProvider
    {
        IRabbitMQConsumingQueueBindingConfiguration Get(Type type);
    }
}