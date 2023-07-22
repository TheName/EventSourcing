using System;
using EventSourcing.Abstractions.DependencyInjection;

namespace EventSourcing.Extensions.DependencyInjection.Bus.RabbitMQ
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds bus layer using RabbitMQ for EventSourcing library.
        /// </summary>
        /// <param name="eventSourcingBuilder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <param name="options">
        /// The <see cref="EventSourcingBusBuilderOptions"/>
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingBuilder WithRabbitMQBus(
            this IEventSourcingBuilder eventSourcingBuilder,
            EventSourcingBusBuilderOptions options = null)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            return eventSourcingBuilder
                .WithBus(options)
                .UsingRabbitMQ();
        }
    }
}