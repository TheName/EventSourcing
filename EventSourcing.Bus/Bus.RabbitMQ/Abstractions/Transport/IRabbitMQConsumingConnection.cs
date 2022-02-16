namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConsumingConnection
    {
        IRabbitMQConsumingChannel CreateConsumingChannel();
    }
}