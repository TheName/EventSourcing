﻿using System;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.ForgettablePayloads.Persistence.PostgreSql.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingForgettablePayloadsPersistenceBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingForgettablePayloadsPersistenceBuilderExtensions
    {
        /// <summary>
        /// Adds PostgreSql Persistence implementation for EventSourcing.ForgettablePayloads.Persistence library.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IEventSourcingForgettablePayloadsPersistenceBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingForgettablePayloadsPersistenceBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="builder"/> is null.
        /// </exception>
        public static IEventSourcingForgettablePayloadsPersistenceBuilder WithForgettablePayloadsPostgreSqlPersistence(
            this IEventSourcingForgettablePayloadsPersistenceBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            builder.Services
                .AddTransient<IForgettablePayloadStorageRepository, ForgettablePayloadStoragePostgreSqlRepository>();

            builder.Services
                .AddConfiguration<IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration, PostgreSqlEventStreamForgettablePayloadPersistenceConfiguration>();

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