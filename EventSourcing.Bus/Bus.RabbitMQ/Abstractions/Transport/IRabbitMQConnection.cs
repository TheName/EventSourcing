namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConnection
    {
        IRabbitMQChannel CreateChannel();
    }
}