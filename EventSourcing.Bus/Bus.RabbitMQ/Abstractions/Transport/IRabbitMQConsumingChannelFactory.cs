namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConsumingChannelFactory
    {
        IRabbitMQConsumingChannel Create();
    }
}