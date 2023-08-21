using System;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Persistence.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default Persistence implementation for EventSourcing.ForgettablePayloads library.
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
        public static IServiceCollection WithForgettablePayloadsPersistence(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddTransient<IForgettablePayloadStorageReader, ForgettablePayloadStorageReader>()
                .AddTransient<IForgettablePayloadStorageWriter, ForgettablePayloadStorageWriter>();

            return serviceCollection;
        }
    }
}
