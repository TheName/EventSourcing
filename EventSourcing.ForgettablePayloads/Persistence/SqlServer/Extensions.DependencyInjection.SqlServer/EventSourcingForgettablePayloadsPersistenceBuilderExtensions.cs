using System;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Persistence.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence.SqlServer
{
    /// <summary>
    /// The <see cref="IEventSourcingForgettablePayloadsPersistenceBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingForgettablePayloadsPersistenceBuilderExtensions
    {
        /// <summary>
        /// Adds SqlServer Persistence implementation for EventSourcing.ForgettablePayloads.Persistence library.
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
        public static IEventSourcingForgettablePayloadsPersistenceBuilder WithForgettablePayloadsSqlServerPersistence(
            this IEventSourcingForgettablePayloadsPersistenceBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            builder.Services
                .AddTransient<IForgettablePayloadStorageRepository, ForgettablePayloadStorageSqlServerRepository>();

            builder.Services
                .AddConfiguration<ISqlServerEventStreamForgettablePayloadPersistenceConfiguration, SqlServerEventStreamForgettablePayloadPersistenceConfiguration>();

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