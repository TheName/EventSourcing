using System;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Configurations;
using EventSourcing.Bus.RabbitMQ.Factories;
using EventSourcing.Bus.RabbitMQ.Helpers;
using EventSourcing.Bus.RabbitMQ.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            eventSourcingBuilder.Services
                .AddOptions<RabbitMQConfiguration>()
                .BindConfiguration(nameof(RabbitMQConfiguration))
                .Validate(
                    configuration => !string.IsNullOrWhiteSpace(configuration.ConnectionString),
                    "Provided connection string is invalid");

            eventSourcingBuilder.Services
                .TryAddTransient<IRabbitMQConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value);

            eventSourcingBuilder.Services
                .AddTransient<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>()
                .AddTransient<IRabbitMQChannelFactory, RabbitMQChannelFactory>()
                .AddTransient<IRabbitMQConnectionFactoryProvider, RabbitMQConnectionFactoryProvider>()
                .AddTransient<IRabbitMQConfigurationProvider, RabbitMQConfigurationProvider>()
                .AddSingleton<IRabbitMQChannelProvider, RabbitMQChannelProvider>()
                .AddSingleton<IRabbitMQConnectionProvider, RabbitMQConnectionProvider>()
                .AddSingleton<IRabbitMQPublishAcknowledgmentTracker, RabbitMQPublishAcknowledgmentTracker>()
                .AddTransient<IEventSourcingBusPublisher, RabbitMQEventSourcingBusPublisher>();

            return eventSourcingBuilder;
        }
    }
}