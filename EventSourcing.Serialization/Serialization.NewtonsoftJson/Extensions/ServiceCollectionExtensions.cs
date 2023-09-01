using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds newtonsoft.json serialization for EventSourcing library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="configureJsonSerializerSettings">
        /// The action that allows to modify <see cref="JsonSerializerSettings"/>
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection WithNewtonsoftJsonSerialization(
            this IServiceCollection serviceCollection,
            Action<JsonSerializerSettings> configureJsonSerializerSettings = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var settings = NewtonsoftJsonSerializer.DefaultSerializerSettings;
            configureJsonSerializerSettings?.Invoke(settings);

            serviceCollection
                .AddTransient<ISerializer>(_ => new NewtonsoftJsonSerializer(settings));

            return serviceCollection;
        }
    }
}
