using System;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Configurations;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Factories
{
    internal class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        private readonly IRabbitMQConfiguration _rabbitMQConfiguration;
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;

        public RabbitMQConnectionFactory(
            IRabbitMQConfiguration rabbitMQConfiguration,
            IEventSourcingConfiguration eventSourcingConfiguration)
        {
            _rabbitMQConfiguration = rabbitMQConfiguration ?? throw new ArgumentNullException(nameof(rabbitMQConfiguration));
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
        }
        
        public IConnection Create()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_rabbitMQConfiguration.ConnectionString),
                ClientProvidedName = $"bounded-context: {_eventSourcingConfiguration.BoundedContext}",
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };

            return factory.CreateConnection();
        }
    }
}