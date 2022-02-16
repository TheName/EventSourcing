using System;
using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQProducingQueueBindingConfigurationProvider
    {
        IRabbitMQProducingQueueBindingConfiguration Get(Type type);
    }
}