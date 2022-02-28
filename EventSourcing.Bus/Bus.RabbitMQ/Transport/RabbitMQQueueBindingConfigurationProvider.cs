using System;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQQueueBindingConfigurationProvider : 
        IRabbitMQConsumingQueueBindingConfigurationProvider,
        IRabbitMQProducingQueueBindingConfigurationProvider
    {
        private const string DirectExchangeType = "direct";
        
        private readonly IEventSourcingConfiguration _eventSourcingConfiguration;
        private readonly IEventSourcingBusHandlingExceptionPublisherConfiguration _exceptionPublisherConfiguration;

        public RabbitMQQueueBindingConfigurationProvider(
            IEventSourcingConfiguration eventSourcingConfiguration,
            IEventSourcingBusHandlingExceptionPublisherConfiguration exceptionPublisherConfiguration)
        {
            _eventSourcingConfiguration = eventSourcingConfiguration ?? throw new ArgumentNullException(nameof(eventSourcingConfiguration));
            _exceptionPublisherConfiguration = exceptionPublisherConfiguration ?? throw new ArgumentNullException(nameof(exceptionPublisherConfiguration));
        }
        
        IRabbitMQConsumingQueueBindingConfiguration IRabbitMQConsumingQueueBindingConfigurationProvider.Get(Type type)
        {
            if (type == typeof(EventStreamEntry))
            {
                return new RabbitMQConsumingQueueBindingConfiguration
                {
                    ExchangeName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                    ExchangeType = DirectExchangeType,
                    PrefetchCount = 100,
                    QueueName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                    RoutingKey = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}"
                };
            }
            
            throw new NotSupportedException($"Producing queue binding configuration for type {type} is not supported");
        }

        IRabbitMQProducingQueueBindingConfiguration IRabbitMQProducingQueueBindingConfigurationProvider.Get(Type type)
        {
            if (type == typeof(EventStreamEntry))
            {
                return new RabbitMQProducingQueueBindingConfiguration
                {
                    ExchangeName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                    ExchangeType = DirectExchangeType,
                    QueueName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                    RoutingKey = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}",
                    PublishingTimeout = TimeSpan.FromSeconds(15)
                };
            }

            if (type == typeof(EventStreamEntryHandlingException))
            {
                return new RabbitMQProducingQueueBindingConfiguration
                {
                    ExchangeName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}.ErrorQueue",
                    ExchangeType = DirectExchangeType,
                    QueueName = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}.ErrorQueue",
                    RoutingKey = $"EventSourcing.{_eventSourcingConfiguration.BoundedContext}.ErrorQueue",
                    PublishingTimeout = _exceptionPublisherConfiguration.PublishingTimeout
                };
            }

            throw new NotSupportedException($"Producing queue binding configuration for type {type} is not supported");
        }
    }
}