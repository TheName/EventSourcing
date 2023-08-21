using System;
using EventSourcing.ForgettablePayloads.Persistence.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.ForgettablePayloads.Persistence.SqlServer.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SqlServer Persistence implementation for EventSourcing.ForgettablePayloads.Persistence library.
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
        public static IServiceCollection WithForgettablePayloadsSqlServerPersistence(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.WithForgettablePayloadsPersistence();

            serviceCollection
                .AddTransient<IForgettablePayloadStorageRepository, ForgettablePayloadStorageSqlServerRepository>();

            serviceCollection
                .AddConfiguration<ISqlServerEventStreamForgettablePayloadPersistenceConfiguration, SqlServerEventStreamForgettablePayloadPersistenceConfiguration>();

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
