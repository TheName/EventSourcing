using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Providers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Factories
{
    internal class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        private readonly IRabbitMQConfigurationProvider _rabbitMQConfigurationProvider;
        private readonly IRabbitMQConnectionFactoryProvider _connectionFactoryProvider;

        public RabbitMQConnectionFactory(
            IRabbitMQConfigurationProvider rabbitMQConfigurationProvider,
            IRabbitMQConnectionFactoryProvider connectionFactoryProvider)
        {
            _rabbitMQConfigurationProvider = rabbitMQConfigurationProvider ?? throw new ArgumentNullException(nameof(rabbitMQConfigurationProvider));
            _connectionFactoryProvider = connectionFactoryProvider ?? throw new ArgumentNullException(nameof(connectionFactoryProvider));
        }
        
        public IConnection Create()
        {
            var factory = _connectionFactoryProvider.Get();
            factory.Uri = new Uri(_rabbitMQConfigurationProvider.ConnectionString);
            factory.ClientProvidedName = _rabbitMQConfigurationProvider.ClientProvidedName;

            return factory.CreateConnection();
        }
    }
}