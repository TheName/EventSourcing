using System;
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
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds aggregates layer for EventSourcing library.
        /// </summary>
        /// <param name="serviceCollection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceCollection"/> is null.
        /// </exception>
        public static IServiceCollection WithAggregates(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddTransient<IEventStreamAggregateBuilder, EventStreamAggregateBuilder>()
                .AddTransient<IEventStreamAggregateConverter, EventStreamAggregateConverter>()
                .AddTransient<IEventStreamAggregateFactory, EventStreamAggregateFactory>()
                .AddTransient<IEventStreamAggregatePublisher, EventStreamAggregatePublisher>()
                .AddTransient<IEventStreamAggregateRetriever, EventStreamAggregateRetriever>()
                .AddTransient<IAggregateEventStore, AggregateEventStore>();

            return serviceCollection;
        }
    }
}