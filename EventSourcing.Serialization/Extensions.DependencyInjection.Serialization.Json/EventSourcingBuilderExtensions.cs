using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Extensions.DependencyInjection.Serialization.Json
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds json serialization for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingSerializationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingSerializationBuilder WithJsonSerialization(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            return eventSourcingBuilder
                .WithSerialization()
                .AddJsonSerializer();
        }
    }
}