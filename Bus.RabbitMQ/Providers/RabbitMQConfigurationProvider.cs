using System;
using EventSourcing.Abstractions.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQConfigurationProvider : IRabbitMQConfigurationProvider
    {
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;

        public string ExchangeName => DefaultName;

        public string QueueName => DefaultName;

        public string RoutingKey => DefaultName;

        private string DefaultName => $"{_eventSourcingConfiguration.BoundedContext}.EventSourcing";

        public RabbitMQConfigurationProvider(
            IEventSourcingConfiguration eventSourcingConfiguration)
        {
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
        }
    }
}