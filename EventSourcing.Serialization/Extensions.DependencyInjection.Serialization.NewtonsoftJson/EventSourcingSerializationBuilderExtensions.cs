using System;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.Abstractions.DependencyInjection;
using EventSourcing.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventSourcing.Extensions.DependencyInjection.Serialization.NewtonsoftJson
{
    /// <summary>
    /// The <see cref="IEventSourcingSerializationBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingSerializationBuilderExtensions
    {
        /// <summary>
        /// Adds newtonsoft.json serialization for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingSerializationBuilder">
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </param>
        /// <param name="configureJsonSerializerSettings">
        /// The action that allows to modify <see cref="JsonSerializerSettings"/>
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingSerializationBuilder"/> is null.
        /// </exception>
        public static IEventSourcingSerializationBuilder AddNewtonsoftJsonSerializer(
            this IEventSourcingSerializationBuilder eventSourcingSerializationBuilder,
            Action<JsonSerializerSettings> configureJsonSerializerSettings = null)
        {
            if (eventSourcingSerializationBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingSerializationBuilder));
            }

            var settings = NewtonsoftJsonSerializer.DefaultSerializerSettings;
            configureJsonSerializerSettings?.Invoke(settings);
            
            eventSourcingSerializationBuilder.Services
                .AddTransient<ISerializer>(_ => new NewtonsoftJsonSerializer(settings));

            return eventSourcingSerializationBuilder;
        }
    }
}