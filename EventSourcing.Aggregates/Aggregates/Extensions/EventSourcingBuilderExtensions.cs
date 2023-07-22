using System;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Abstractions.Builders;
using EventSourcing.Aggregates.Abstractions.Conversion;
using EventSourcing.Aggregates.Abstractions.Factories;
using EventSourcing.Aggregates.Abstractions.Publishers;
using EventSourcing.Aggregates.Abstractions.Retrievers;
using EventSourcing.Aggregates.Builders;
using EventSourcing.Aggregates.Conversion;
using EventSourcing.Aggregates.Factories;
using EventSourcing.Aggregates.Publishers;
using EventSourcing.Aggregates.Retrievers;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Aggregates.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds aggregates layer for EventSourcing library.
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
        public static IEventSourcingBuilder WithAggregates(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.Services
                .AddTransient<IEventStreamAggregateBuilder, EventStreamAggregateBuilder>()
                .AddTransient<IEventStreamAggregateConverter, EventStreamAggregateConverter>()
                .AddTransient<IEventStreamAggregateFactory, EventStreamAggregateFactory>()
                .AddTransient<IEventStreamAggregatePublisher, EventStreamAggregatePublisher>()
                .AddTransient<IEventStreamAggregateRetriever, EventStreamAggregateRetriever>()
                .AddTransient<IAggregateEventStore, AggregateEventStore>();

            return eventSourcingBuilder;
        }
    }
}