namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQHandlingExceptionProducingChannelFactory
    {
        IRabbitMQProducingChannel Create();
    }
}