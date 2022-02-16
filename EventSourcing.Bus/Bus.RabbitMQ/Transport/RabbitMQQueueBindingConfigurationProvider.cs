using System;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQQueueBindingConfigurationProvider : 
        IRabbitMQConsumingQueueBindingConfigurationProvider,
        IRabbitMQProducingQueueBindingConfigurationProvider
    {
        private const string DirectExchangeType = "direct";
        
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;

        public RabbitMQQueueBindingConfigurationProvider(IEventSourcingConfiguration eventSourcingConfiguration)
        {
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
        }
        
        IRabbitMQConsumingQueueBindingConfiguration IRabbitMQConsumingQueueBindingConfigurationProvider.Get(Type type) =>
            new RabbitMQConsumingQueueBindingConfiguration
            {
                ExchangeName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                ExchangeType = DirectExchangeType,
                PrefetchCount = 100,
                QueueName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                RoutingKey = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}"
            };

        IRabbitMQProducingQueueBindingConfiguration IRabbitMQProducingQueueBindingConfigurationProvider.Get(Type type) =>
            new RabbitMQProducingQueueBindingConfiguration
            {
                ExchangeName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                ExchangeType = DirectExchangeType,
                QueueName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                RoutingKey = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                PublishingTimeout = TimeSpan.FromSeconds(15)
            };
    }
}