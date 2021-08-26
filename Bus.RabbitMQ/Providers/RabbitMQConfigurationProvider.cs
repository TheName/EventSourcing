using System;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQConfigurationProvider : IRabbitMQConfigurationProvider
    {
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;
        private readonly IRabbitMQConfiguration _rabbitMQConfiguration;

        public string ExchangeName => DefaultName;
        
        public string ExchangeType => "direct";

        public string QueueName => DefaultName;

        public string RoutingKey => DefaultName;

        public string ClientProvidedName => $"bounded-context: {_eventSourcingConfiguration.BoundedContext}";
        
        public string ConnectionString => _rabbitMQConfiguration.ConnectionString;
        
        public TimeSpan PublishingTimeout => TimeSpan.FromSeconds(9);

        private string DefaultName => $"{_eventSourcingConfiguration.BoundedContext}.EventSourcing";

        public RabbitMQConfigurationProvider(
            IEventSourcingConfiguration eventSourcingConfiguration,
            IRabbitMQConfiguration rabbitMQConfiguration)
        {
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
            _rabbitMQConfiguration = rabbitMQConfiguration ?? throw new ArgumentNullException(nameof(rabbitMQConfiguration));
        }
    }
}