using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.Reconciliation;
using EventSourcing.Configurations;
using EventSourcing.Conversion;
using EventSourcing.Handling;
using EventSourcing.Reconciliation;
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
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection AddEventSourcing(this IServiceCollection serviceCollection)
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

            return serviceCollection;
        }
    }
}