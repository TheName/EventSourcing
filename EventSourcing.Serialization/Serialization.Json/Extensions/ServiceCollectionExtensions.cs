using System;
using System.Text.Json;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Serialization.Json.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds json serialization for EventSourcing library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="configureJsonSerializerOptions">
        /// The action that allows to modify <see cref="JsonSerializerOptions"/>
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection WithJsonSerialization(
            this IServiceCollection serviceCollection,
            Action<JsonSerializerOptions> configureJsonSerializerOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .WithSerialization()
                .AddJsonSerializer(configureJsonSerializerOptions);
        }

        private static IServiceCollection AddJsonSerializer(
            this IServiceCollection serviceCollection,
            Action<JsonSerializerOptions> configureJsonSerializerOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var jsonSerializerOptions = JsonSerializer.DefaultJsonSerializerOptions;
            configureJsonSerializerOptions?.Invoke(jsonSerializerOptions);

            serviceCollection
                .AddTransient<ISerializer>(_ => new JsonSerializer(jsonSerializerOptions));

            return serviceCollection;
        }
    }
}
