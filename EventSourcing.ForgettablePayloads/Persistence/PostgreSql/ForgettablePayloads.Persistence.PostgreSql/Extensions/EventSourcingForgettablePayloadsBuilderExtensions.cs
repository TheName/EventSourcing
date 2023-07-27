using System;
using EventSourcing.ForgettablePayloads.Abstractions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Persistence.Extensions;

namespace EventSourcing.ForgettablePayloads.Persistence.PostgreSql.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingForgettablePayloadsBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingForgettablePayloadsBuilderExtensions
    {
        /// <summary>
        /// Adds PostgreSql Persistence implementation for EventSourcing.ForgettablePayloads.Persistence library.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IEventSourcingForgettablePayloadsBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingForgettablePayloadsBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="builder"/> is null.
        /// </exception>
        public static IEventSourcingForgettablePayloadsBuilder WithForgettablePayloadsPostgreSqlPersistence(
            this IEventSourcingForgettablePayloadsBuilder builder)
        {
            return builder
                .WithForgettablePayloadsPersistence()
                .WithForgettablePayloadsPostgreSqlPersistence();
        }
    }
}