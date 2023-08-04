using System;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.Serialization.Abstractions.DependencyInjection;
using EventSourcing.Serialization.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventSourcing.Extensions.DependencyInjection.Serialization.NewtonsoftJson
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds newtonsoft.json serialization for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <param name="configureJsonSerializerSettings">
        /// The action that allows to modify <see cref="JsonSerializerSettings"/>
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingSerializationBuilder WithNewtonsoftJsonSerialization(
            this IEventSourcingBuilder eventSourcingBuilder,
            Action<JsonSerializerSettings> configureJsonSerializerSettings = null)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            return eventSourcingBuilder
                .WithSerialization()
                .AddNewtonsoftJsonSerializer(configureJsonSerializerSettings);
        }
    }
}