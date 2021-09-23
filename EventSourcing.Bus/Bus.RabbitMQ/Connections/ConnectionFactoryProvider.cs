using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Connections
{
    internal class ConnectionFactoryProvider : IConnectionFactoryProvider
    {
        private const int ConsumerDispatchConcurrency = 50;
        
        private readonly IRabbitMQConfiguration _configuration;

        public ConnectionFactoryProvider(IRabbitMQConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public IConnectionFactory Get()
        {
            return new ConnectionFactory
            {
                DispatchConsumersAsync = true,
                ConsumerDispatchConcurrency = ConsumerDispatchConcurrency,
                Uri = new Uri(_configuration.ConnectionString)
            };
        }
    }
}