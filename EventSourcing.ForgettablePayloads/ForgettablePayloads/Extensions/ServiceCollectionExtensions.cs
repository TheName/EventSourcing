using System;
using EventSourcing.ForgettablePayloads.Cleanup;
using EventSourcing.ForgettablePayloads.Configurations;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.Hooks;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.Hooks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.ForgettablePayloads.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default implementation for EventSourcing.ForgettablePayloads library.
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
        public static IServiceCollection WithForgettablePayloads(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddTransient<IForgettablePayloadClaimingService, ForgettablePayloadClaimingService>()
                .AddTransient<IForgettablePayloadDescriptorLoader, ForgettablePayloadDescriptorLoader>()
                .AddTransient<IForgettablePayloadEventStreamForgettingService, ForgettablePayloadEventStreamForgettingService>()
                .AddTransient<IForgettablePayloadFinder, ForgettablePayloadFinder>()
                .AddTransient<IForgettablePayloadForgettingService, ForgettablePayloadForgettingService>()
                .AddTransient<IForgettablePayloadContentConverter, ForgettablePayloadContentConverter>()
                .AddTransient<IForgettablePayloadTypeIdentifierConverter, ForgettablePayloadTypeIdentifierConverter>()
                .AddSingleton<IForgettablePayloadTypeIdentifierConverterProvider, ForgettablePayloadTypeIdentifierConverterProvider>()
                .AddTransient<IUnclaimedForgettablePayloadsCleanupJob, UnclaimedForgettablePayloadsCleanupJob>()
                .AddHostedService<UnclaimedForgettablePayloadsCleanupBackgroundService>();

            serviceCollection
                .AddTransient<IEventStreamEventDescriptorPostDeserializationHook, AssignForgettablePayloadServicesPostDeserializationHook>()
                .AddTransient<IEventStreamEventWithMetadataPrePublishingHook, StoreForgettablePayloadsPrePublishingHook>();

            serviceCollection
                .AddConfiguration<IForgettablePayloadTypeConversionConfiguration, ForgettablePayloadTypeConversionConfiguration>()
                .AddConfiguration<IForgettablePayloadConfiguration, ForgettablePayloadConfiguration>()
                .PostConfigure<ForgettablePayloadConfiguration>(configuration =>
                {
                    if (configuration.UnclaimedForgettablePayloadsCleanupTimeout == TimeSpan.Zero)
                    {
                        configuration.UnclaimedForgettablePayloadsCleanupTimeout = TimeSpan.MaxValue;
                    }

                    if (configuration.UnclaimedForgettablePayloadsCleanupJobInterval == TimeSpan.Zero)
                    {
                        configuration.UnclaimedForgettablePayloadsCleanupJobInterval = TimeSpan.FromDays(1);
                    }
                });

            return serviceCollection;
        }

        private static IServiceCollection AddConfiguration<TInterface, TConfiguration>(
            this IServiceCollection serviceCollection) where TConfiguration : class, TInterface where TInterface : class
        {
            serviceCollection
                .AddOptions<TConfiguration>()
                .BindConfiguration(typeof(TConfiguration).Name);

            serviceCollection
                .TryAddTransient<TInterface>(provider =>
                    provider.GetRequiredService<IOptions<TConfiguration>>().Value);

            return serviceCollection;
        }
    }
}