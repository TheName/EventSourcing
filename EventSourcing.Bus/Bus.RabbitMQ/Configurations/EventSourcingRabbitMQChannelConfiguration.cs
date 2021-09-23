using System;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal class EventSourcingRabbitMQChannelConfiguration : IRabbitMQPublishingChannelConfiguration, IRabbitMQConsumingChannelConfiguration
    {
        private const string DefaultExchangeType = "direct";
        
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;
        
        public TimeSpan PublishingTimeout => TimeSpan.FromSeconds(9);
        public string ExchangeName => DefaultName;
        public string QueueName => DefaultName;
        public string RoutingKey => DefaultName;
        public string ExchangeType => DefaultExchangeType;
        public ushort PrefetchCount => 100;

        private string DefaultName => $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}";

        public EventSourcingRabbitMQChannelConfiguration(IEventSourcingConfiguration eventSourcingConfiguration)
        {
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
        }
    }
}