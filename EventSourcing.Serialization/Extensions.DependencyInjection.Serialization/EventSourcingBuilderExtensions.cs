using System;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.Serialization;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Extensions.DependencyInjection.Serialization
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds serialization layer for EventSourcing library.
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
        public static IEventSourcingSerializationBuilder WithSerialization(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.Services
                .AddOptions<EventSourcingSerializationConfiguration>()
                .BindConfiguration(nameof(EventSourcingSerializationConfiguration));

            eventSourcingBuilder.Services
                .TryAddTransient<IEventSourcingSerializationConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingSerializationConfiguration>>().Value);
            
            eventSourcingBuilder.Services.TryAddTransient<ISerializerProvider, SerializerProvider>();

            return new EventSourcingSerializationBuilder(eventSourcingBuilder);
        }
    }
}