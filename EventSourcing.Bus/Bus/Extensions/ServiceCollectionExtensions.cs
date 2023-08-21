using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Bus.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds bus layer for EventSourcing library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="options">
        /// The <see cref="EventSourcingBusBuilderOptions"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection WithBus(
            this IServiceCollection serviceCollection,
            EventSourcingBusBuilderOptions options = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var optionsToUse = options ?? new EventSourcingBusBuilderOptions();

            if (optionsToUse.WithConsumer)
            {
                serviceCollection.AddHostedService<EventSourcingConsumerHostedService>();
            }

            serviceCollection
                .AddOptions<EventSourcingBusHandlingExceptionPublisherConfiguration>()
                .BindConfiguration(nameof(EventSourcingBusHandlingExceptionPublisherConfiguration));
                // .Validate(
                //     configuration => configuration.PublishingTimeout <= TimeSpan.Zero,
                //     "PublishingTimeout cannot be less or equal to zero.");

            serviceCollection
                .TryAddTransient<IEventSourcingBusHandlingExceptionPublisherConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingBusHandlingExceptionPublisherConfiguration>>().Value);

            return serviceCollection;
        }
    }
}