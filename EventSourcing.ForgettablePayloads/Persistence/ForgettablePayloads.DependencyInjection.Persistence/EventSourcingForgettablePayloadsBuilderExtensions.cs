﻿using System;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence
{
    /// <summary>
    /// The <see cref="IEventSourcingForgettablePayloadsBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingForgettablePayloadsBuilderExtensions
    {
        /// <summary>
        /// Adds default Persistence implementation for EventSourcing.ForgettablePayloads library.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IEventSourcingForgettablePayloadsBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingForgettablePayloadsPersistenceBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="builder"/> is null.
        /// </exception>
        public static IEventSourcingForgettablePayloadsPersistenceBuilder WithForgettablePayloadsPersistence(
            this IEventSourcingForgettablePayloadsBuilder builder)
        {
            var persistenceBuilder = new EventSourcingForgettablePayloadsPersistenceBuilder(builder);

            persistenceBuilder.Services
                .AddTransient<IForgettablePayloadStorageReader, ForgettablePayloadStorageReader>()
                .AddTransient<IForgettablePayloadStorageWriter, ForgettablePayloadStorageWriter>();

            return persistenceBuilder;
        }
    }
}