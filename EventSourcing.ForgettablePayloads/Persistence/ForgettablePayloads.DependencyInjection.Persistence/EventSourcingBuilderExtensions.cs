using System;
using EventSourcing.Abstractions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds default implementation for EventSourcing.ForgettablePayloads library with default Persistence implementation
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingForgettablePayloadsPersistenceBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingForgettablePayloadsPersistenceBuilder WithForgettablePayloadsAndPersistence(
            this IEventSourcingBuilder eventSourcingBuilder)
        {
            return eventSourcingBuilder
                .WithForgettablePayloads()
                .WithForgettablePayloadsPersistence();
        }
    }
}