﻿using System;
using EventSourcing.Abstractions.Hooks;
using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Abstractions.Cleanup;
using EventSourcing.ForgettablePayloads.Abstractions.Configurations;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Cleanup;
using EventSourcing.ForgettablePayloads.Configurations;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.Hooks;
using EventSourcing.ForgettablePayloads.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds default implementation for EventSourcing.ForgettablePayloads library.
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingForgettablePayloadsBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingForgettablePayloadsBuilder WithForgettablePayloads(this IEventSourcingBuilder eventSourcingBuilder)
        {
            var builder = new EventSourcingForgettablePayloadsBuilder(eventSourcingBuilder);

            builder.Services
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

            builder.Services
                .AddTransient<IEventStreamEventDescriptorPostDeserializationHook, AssignForgettablePayloadServicesPostDeserializationHook>()
                .AddTransient<IEventStreamEventWithMetadataPrePublishingHook, StoreForgettablePayloadsPrePublishingHook>();

            builder.Services
                .AddConfiguration<IForgettablePayloadTypeConversionConfiguration, ForgettablePayloadTypeConversionConfiguration>()
                .AddConfiguration<IForgettablePayloadConfiguration, ForgettablePayloadConfiguration>();

            return builder;
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