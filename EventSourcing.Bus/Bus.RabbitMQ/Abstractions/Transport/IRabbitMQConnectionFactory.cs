using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal interface IRabbitMQConnectionFactory
    {
        IRabbitMQConnection Create(IRabbitMQConnectionConfiguration connectionConfiguration);
    }
}