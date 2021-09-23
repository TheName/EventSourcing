using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Connections
{
    internal interface IRabbitMQConnection
    {
        IRabbitMQPublishingChannel CreatePublishingChannel(IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration);
        
        IRabbitMQConsumingChannel CreateConsumingChannel(IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration);
    }
}