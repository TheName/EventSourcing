using System;
using EventSourcing.Bus;
using EventSourcing.Bus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Extensions.DependencyInjection.Bus
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds bus layer for EventSourcing library.
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
        public static IEventSourcingBuilder WithBus(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.Services.AddHostedService<EventSourcingConsumerHostedService>();
            
            eventSourcingBuilder.Services
                .AddOptions<EventSourcingBusHandlingExceptionPublisherConfiguration>()
                .BindConfiguration(nameof(EventSourcingBusHandlingExceptionPublisherConfiguration))
                .Validate(
                    configuration => configuration.PublishingTimeout <= TimeSpan.Zero,
                    "PublishingTimeout cannot be less or equal to zero.");

            eventSourcingBuilder.Services
                .TryAddTransient<IEventSourcingBusHandlingExceptionPublisherConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingBusHandlingExceptionPublisherConfiguration>>().Value);

            return eventSourcingBuilder;
        }
    }
}