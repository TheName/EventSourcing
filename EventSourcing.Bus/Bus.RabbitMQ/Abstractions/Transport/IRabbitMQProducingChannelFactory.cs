namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQProducingChannelFactory
    {
        IRabbitMQProducingChannel Create();
    }
}