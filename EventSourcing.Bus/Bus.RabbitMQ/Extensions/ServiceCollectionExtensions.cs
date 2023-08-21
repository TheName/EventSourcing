using System;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Configurations;
using EventSourcing.Bus.RabbitMQ.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcing.Bus.RabbitMQ.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Makes bus layer using RabbitMQ for EventSourcing library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection UsingRabbitMQ(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            
            serviceCollection
                .AddOptions<RabbitMQConfiguration>()
                .BindConfiguration(nameof(RabbitMQConfiguration))
                .Validate(
                    configuration => !string.IsNullOrWhiteSpace(configuration.ConnectionString),
                    "Provided connection string is invalid");

            // common
            serviceCollection
                .AddTransient<IRabbitMQConfiguration>(provider => provider.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value)
                .AddTransient<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>()
                .AddTransient<IRabbitMQConnectionConfigurationProvider, RabbitMQConnectionConfigurationProvider>();

            // consuming
            serviceCollection
                .AddTransient<IRabbitMQConsumerFactory, RabbitMQConsumerFactory>()
                .AddTransient<IRabbitMQConsumingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQConsumingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusConsumer, RabbitMQEventSourcingBusConsumer>();

            // producing
            serviceCollection
                .AddTransient<IRabbitMQProducerFactory, RabbitMQProducerFactory>()
                .AddTransient<IRabbitMQProducingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQProducingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusPublisher, RabbitMQEventSourcingBusPublisher>();

            // handling exception producing
            serviceCollection
                .AddTransient<IRabbitMQHandlingExceptionProducerFactory, RabbitMQHandlingExceptionProducerFactory>()
                .AddTransient<IRabbitMQProducingQueueBindingConfigurationProvider, RabbitMQQueueBindingConfigurationProvider>()
                .AddSingleton<IRabbitMQHandlingExceptionProducingChannelFactory, RabbitMQChannelFactory>()
                .AddSingleton<IEventSourcingBusHandlingExceptionPublisher, EventSourcingBusHandlingExceptionPublisher>();

            return serviceCollection;
        }
    }
}