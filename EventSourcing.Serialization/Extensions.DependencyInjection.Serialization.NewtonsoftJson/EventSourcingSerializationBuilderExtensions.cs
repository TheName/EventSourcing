using System;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;

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
        /// <returns>
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingSerializationBuilder"/> is null.
        /// </exception>
        public static IEventSourcingSerializationBuilder AddNewtonsoftJsonSerializer(this IEventSourcingSerializationBuilder eventSourcingSerializationBuilder)
        {
            if (eventSourcingSerializationBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingSerializationBuilder));
            }
            
            eventSourcingSerializationBuilder.Services
                .AddTransient<ISerializer, NewtonsoftJsonSerializer>();

            return eventSourcingSerializationBuilder;
        }
    }
}