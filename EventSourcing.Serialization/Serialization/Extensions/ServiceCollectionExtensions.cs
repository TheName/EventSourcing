using System;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Serialization.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds serialization layer for EventSourcing library.
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
        public static IServiceCollection WithSerialization(this IServiceCollection serviceCollection)
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
