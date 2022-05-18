using System;
using EventSourcing.Extensions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence.PostgreSql
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds EventSourcing.ForgettablePayloads library with default implementations and PostgreSql persistence implementation
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEventSourcingBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="builder"/> is null.
        /// </exception>
        public static IEventSourcingBuilder WithForgettablePayloadsUsingPostgreSqlPersistence(
            this IEventSourcingBuilder builder)
        {
            return builder
                .WithForgettablePayloads()
                .WithForgettablePayloadsPostgreSqlPersistence();
        }
    }
}