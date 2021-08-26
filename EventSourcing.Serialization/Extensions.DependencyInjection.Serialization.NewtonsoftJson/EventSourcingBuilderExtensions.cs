using System;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Extensions.DependencyInjection.Serialization.NewtonsoftJson
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds default newtonsoft.json serialization for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingBuilder WithNewtonsoftJsonSerialization(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.Services
                .AddTransient<ISerializer, NewtonsoftJsonSerializer>();

            return eventSourcingBuilder;
        }
    }
}