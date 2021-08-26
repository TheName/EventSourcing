using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQConnectionFactoryProvider : IRabbitMQConnectionFactoryProvider
    {
        public IConnectionFactory Get()
        {
            return new ConnectionFactory();
        }
    }
}