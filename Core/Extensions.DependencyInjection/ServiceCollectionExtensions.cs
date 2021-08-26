﻿using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Extensions.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default implementation for EventSourcing.Abstractions library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IEventSourcingBuilder AddEventSourcing(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddTransient<IEventStreamPublisher, EventStreamPublisher>();
            
            serviceCollection
                .AddOptions<EventSourcingConfiguration>()
                .BindConfiguration(nameof(EventSourcingConfiguration))
                .Validate(
                    configuration => !string.IsNullOrWhiteSpace(configuration.BoundedContext),
                    "BoundedContext cannot be empty.");

            serviceCollection
                .TryAddTransient<IEventSourcingConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingConfiguration>>().Value);

            return new EventSourcingBuilder(serviceCollection);
        }
    }
}