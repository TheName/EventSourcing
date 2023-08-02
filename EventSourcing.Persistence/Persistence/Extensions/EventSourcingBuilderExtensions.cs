﻿using System;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Persistence.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds default persistence for EventSourcing library.
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
        public static IEventSourcingBuilder WithPersistence(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.Services
                .AddTransient<IEventStreamStagingReader, EventStreamStagingReader>()
                .AddTransient<IEventStreamStagingWriter, EventStreamStagingWriter>()
                .AddTransient<IEventStreamWriter, EventStreamWriter>()
                .AddTransient<IEventStreamReader, EventStreamReader>();

            return eventSourcingBuilder;
        }
    }
}