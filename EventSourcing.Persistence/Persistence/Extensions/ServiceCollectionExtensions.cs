using System;
using EventSourcing.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Persistence.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default persistence for EventSourcing library.
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
        public static IServiceCollection WithPersistence(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddTransient<IEventStreamStagingReader, EventStreamStagingReader>()
                .AddTransient<IEventStreamStagingWriter, EventStreamStagingWriter>()
                .AddTransient<IEventStreamWriter, EventStreamWriter>()
                .AddTransient<IEventStreamReader, EventStreamReader>();

            return serviceCollection;
        }
    }
}
