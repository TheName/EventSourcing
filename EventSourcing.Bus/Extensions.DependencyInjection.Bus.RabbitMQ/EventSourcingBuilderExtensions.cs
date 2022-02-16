using System;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Configurations;
using EventSourcing.Bus.RabbitMQ.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcing.Extensions.DependencyInjection.Bus.RabbitMQ
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds RabbitMQ bus layer for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingBuilder WithRabbitMQBus(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.WithBus();

            eventSourcingBuilder.Services
                .AddOptions<RabbitMQConfiguration>()
                .BindConfiguration(nameof(RabbitMQConfiguration))
                .Validate(
                    configuration => !string.IsNullOrWhiteSpace(configuration.ConnectionString),
                    "Provided connection string is invalid");
            
            // common
            eventSourcingBuilder.Services
                .AddTransient<IRabbitMQConfiguration>(provider => provider.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value)
                .AddTransient<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>()
                .AddTransient<IRabbitMQConnectionConfigurationProvider, RabbitMQConnectionConfigurationProvider>();

            // consuming
            eventSourcingBuilder.Services
                .AddTransient<IRabbitMQConsumerFactory, RabbitMQConsumerFactory>()
                .AddTransient<IRabbitMQConsumingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQConsumingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusConsumer, RabbitMQEventSourcingBusConsumer>();
                
            // producing
            eventSourcingBuilder.Services
                .AddTransient<IRabbitMQProducerFactory, RabbitMQProducerFactory>()
                .AddTransient<IRabbitMQProducingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQProducingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusPublisher, RabbitMQEventSourcingBusPublisher>();

            return eventSourcingBuilder;
        }
    }
}