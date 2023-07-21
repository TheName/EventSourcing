using System;
using EventSourcing.Bus;
using EventSourcing.Bus.Abstractions;
using EventSourcing.DependencyInjection;
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
        /// <param name="options">
        /// The <see cref="EventSourcingBusBuilderOptions"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBusBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="eventSourcingBuilder"/> is null.
        /// </exception>
        public static IEventSourcingBusBuilder WithBus(
            this IEventSourcingBuilder eventSourcingBuilder,
            EventSourcingBusBuilderOptions options = null)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            var optionsToUse = options ?? new EventSourcingBusBuilderOptions();

            if (optionsToUse.WithConsumer)
            {
                eventSourcingBuilder.Services.AddHostedService<EventSourcingConsumerHostedService>();
            }

            eventSourcingBuilder.Services
                .AddOptions<EventSourcingBusHandlingExceptionPublisherConfiguration>()
                .BindConfiguration(nameof(EventSourcingBusHandlingExceptionPublisherConfiguration));
                // .Validate(
                //     configuration => configuration.PublishingTimeout <= TimeSpan.Zero,
                //     "PublishingTimeout cannot be less or equal to zero.");

            eventSourcingBuilder.Services
                .TryAddTransient<IEventSourcingBusHandlingExceptionPublisherConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<EventSourcingBusHandlingExceptionPublisherConfiguration>>().Value);

            return new EventSourcingBusBuilder(
                eventSourcingBuilder,
                optionsToUse);
        }
    }
}