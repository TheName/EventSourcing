using System;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Configurations;
using EventSourcing.Bus.RabbitMQ.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcing.Bus.RabbitMQ.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingBusBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBusBuilderExtensions
    {
        /// <summary>
        /// Makes bus layer using RabbitMQ for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingBusBuilder">
        /// The <see cref="IEventSourcingBusBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBusBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBusBuilder"/> is null.
        /// </exception>
        public static IEventSourcingBusBuilder UsingRabbitMQ(
            this IEventSourcingBusBuilder eventSourcingBusBuilder)
        {
            if (eventSourcingBusBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBusBuilder));
            }
            
            eventSourcingBusBuilder.Services
                .AddOptions<RabbitMQConfiguration>()
                .BindConfiguration(nameof(RabbitMQConfiguration))
                .Validate(
                    configuration => !string.IsNullOrWhiteSpace(configuration.ConnectionString),
                    "Provided connection string is invalid");

            // common
            eventSourcingBusBuilder.Services
                .AddTransient<IRabbitMQConfiguration>(provider => provider.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value)
                .AddTransient<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>()
                .AddTransient<IRabbitMQConnectionConfigurationProvider, RabbitMQConnectionConfigurationProvider>();

            // consuming
            if (eventSourcingBusBuilder.BusBuilderOptions.WithConsumer)
            {
                eventSourcingBusBuilder.Services
                    .AddTransient<IRabbitMQConsumerFactory, RabbitMQConsumerFactory>()
                    .AddTransient<IRabbitMQConsumingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                    .AddSingleton<IRabbitMQConsumingChannelFactory, RabbitMQChannelFactory>()
                    .AddSingleton<IEventSourcingBusConsumer, RabbitMQEventSourcingBusConsumer>();
            }

            // producing
            eventSourcingBusBuilder.Services
                .AddTransient<IRabbitMQProducerFactory, RabbitMQProducerFactory>()
                .AddTransient<IRabbitMQProducingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQProducingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusPublisher, RabbitMQEventSourcingBusPublisher>();

            // handling exception producing
            eventSourcingBusBuilder.Services
                .AddTransient<IRabbitMQHandlingExceptionProducerFactory, RabbitMQHandlingExceptionProducerFactory>()
                .AddTransient<IRabbitMQProducingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQHandlingExceptionProducingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusHandlingExceptionPublisher, EventSourcingBusHandlingExceptionPublisher>();

            return eventSourcingBusBuilder;
        }
    }
}