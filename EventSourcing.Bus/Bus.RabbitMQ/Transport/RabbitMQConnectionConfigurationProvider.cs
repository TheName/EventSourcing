using System;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Configurations;
using EventSourcing.Bus.RabbitMQ.Enums;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQConnectionConfigurationProvider : IRabbitMQConnectionConfigurationProvider
    {
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;
        public IRabbitMQConnectionConfiguration ConsumerConnectionConfiguration { get; }
        public IRabbitMQConnectionConfiguration ProducerConnectionConfiguration { get; }

        public RabbitMQConnectionConfigurationProvider(
            IEventSourcingConfiguration eventSourcingConfiguration,
            IRabbitMQConfiguration rabbitMQConfiguration)
        {
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
            if (rabbitMQConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rabbitMQConfiguration));
            }

            ConsumerConnectionConfiguration = new RabbitMQConnectionConfiguration(
                GetClientName(RabbitMQConnectionType.Consumer),
                TimeSpan.FromSeconds(5),
                new Uri(rabbitMQConfiguration.ConnectionString),
                50);

            ProducerConnectionConfiguration = new RabbitMQConnectionConfiguration(
                GetClientName(RabbitMQConnectionType.Producer),
                TimeSpan.FromSeconds(5),
                new Uri(rabbitMQConfiguration.ConnectionString),
                50);
        }

        private string GetClientName(RabbitMQConnectionType connectionType) =>
            $"{_eventSourcingConfiguration.BoundedContext}_{GetClientNameSuffix(connectionType)}";

        private static string GetClientNameSuffix(RabbitMQConnectionType connectionType)
        {
            switch (connectionType)
            {
                case RabbitMQConnectionType.Consumer:
                case RabbitMQConnectionType.Producer:
                    return connectionType.ToString();
                
                default:
                    throw new NotSupportedException(
                        $"Creating RabbitMQ connection with type {connectionType} is not currently supported.");
            }
        }
    }
}