using System;
using System.Text.Json;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Serialization.Json.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingSerializationBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingSerializationBuilderExtensions
    {
        /// <summary>
        /// Adds json serialization for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingSerializationBuilder">
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </param>
        /// <param name="configureJsonSerializerOptions">
        /// The action that allows to modify <see cref="JsonSerializerOptions"/>
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingSerializationBuilder"/> is null.
        /// </exception>
        public static IEventSourcingSerializationBuilder AddJsonSerializer(
            this IEventSourcingSerializationBuilder eventSourcingSerializationBuilder,
            Action<JsonSerializerOptions> configureJsonSerializerOptions = null)
        {
            if (eventSourcingSerializationBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingSerializationBuilder));
            }

            var jsonSerializerOptions = JsonSerializer.DefaultJsonSerializerOptions;
            configureJsonSerializerOptions?.Invoke(jsonSerializerOptions);
            
            eventSourcingSerializationBuilder.Services
                .AddTransient<ISerializer>(_ => new JsonSerializer(jsonSerializerOptions));

            return eventSourcingSerializationBuilder;
        }
    }
}