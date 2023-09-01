using System;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.Reconciliation;
using EventSourcing.Bus;
using EventSourcing.Configurations;
using EventSourcing.Conversion;
using EventSourcing.Handling;
using EventSourcing.Reconciliation;
using EventSourcing.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default implementation for EventSourcing library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="busOptions">
        /// The <see cref="EventSourcingBusBuilderOptions"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection AddEventSourcing(
            this IServiceCollection serviceCollection,
            EventSourcingBusBuilderOptions busOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddTransient<IEventStreamPublisher, EventStreamPublisher>()
                .AddTransient<IEventStreamEventConverter, EventStreamEventConverter>()
                .AddTransient<IEventStreamEventTypeIdentifierConverter, EventStreamEventTypeIdentifierConverter>()
                .AddSingleton<IEventStreamEventTypeIdentifierConverterProvider, EventStreamEventTypeIdentifierConverterProvider>()
                .AddTransient<IEventHandlerProvider, EventHandlerProvider>()
                .AddTransient<IEventHandlingExceptionsHandler, EventHandlingExceptionsHandler>()
                .AddTransient<IEventStreamEntryDispatcher, EventStreamEntryDispatcher>()
                .AddTransient<IEventStreamRetriever, EventStreamRetriever>()
                .AddTransient<IReconciliationJob, ReconciliationJob>()
                .AddTransient<IEventStreamStagedEntriesReconciliationService, EventStreamStagedEntriesReconciliationService>();

            serviceCollection
                .AddOptions<EventSourcingConfiguration>()
                .BindConfiguration(nameof(EventSourcingConfiguration))
                .Validate(
                    configuration => !string.IsNullOrWhiteSpace(configuration.BoundedContext),
                    "BoundedContext cannot be empty.")
                .PostConfigure(configuration =>
                {
                    if (configuration.ReconciliationJobInterval == TimeSpan.Zero)
                    {
                        configuration.ReconciliationJobInterval = TimeSpan.FromSeconds(30);
                    }

                    if (configuration.ReconciliationJobGracePeriodAfterStagingTime == TimeSpan.Zero)
                    {
                        configuration.ReconciliationJobGracePeriodAfterStagingTime = TimeSpan.FromSeconds(15);
                    }
                });

            serviceCollection
                .TryAddTransient<IEventSourcingConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingConfiguration>>().Value);

            serviceCollection
                .AddOptions<EventSourcingTypeConversionConfiguration>()
                .BindConfiguration(nameof(EventSourcingTypeConversionConfiguration));

            serviceCollection
                .TryAddTransient<IEventSourcingTypeConversionConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingTypeConversionConfiguration>>().Value);

            serviceCollection.AddHostedService<ReconciliationBackgroundService>();

            return serviceCollection
                .WithSerialization()
                .WithBus(busOptions);
        }

        private static IServiceCollection WithBus(
            this IServiceCollection serviceCollection,
            EventSourcingBusBuilderOptions options)
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

        private static IServiceCollection WithSerialization(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddOptions<EventSourcingSerializationConfiguration>()
                .BindConfiguration(nameof(EventSourcingSerializationConfiguration));

            serviceCollection
                .TryAddTransient<IEventSourcingSerializationConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingSerializationConfiguration>>().Value);

            serviceCollection.TryAddTransient<ISerializerProvider, SerializerProvider>();

            return serviceCollection;
        }
    }
}
