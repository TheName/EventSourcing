using System;
using System.Reflection;
using EventSourcing.Persistence;
using EventSourcing.Persistence.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventSourcing.Extensions.DependencyInjection.Persistence.PostgreSql
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds PostgreSQL persistence layer for EventSourcing library.
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
        public static IEventSourcingBuilder WithPostgreSqlPersistence(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.WithPersistence();

            eventSourcingBuilder.Services
                .AddOptions<PostgreSqlEventStreamPersistenceConfiguration>()
                .BindConfiguration(nameof(PostgreSqlEventStreamPersistenceConfiguration))
                .Validate(
                    configuration =>
                    {
                        try
                        {
                            var builder = new NpgsqlConnectionStringBuilder(configuration.ConnectionString);
                            var validateMethod = typeof(NpgsqlConnectionStringBuilder)
                                .GetMethod("Validate", BindingFlags.Instance | BindingFlags.NonPublic);

                            if (validateMethod == null)
                            {
                                throw new Exception();
                            }
                            
                            _ = validateMethod.Invoke(builder, Array.Empty<object>());
                            
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    },
                    "Provided connection string is invalid");

            eventSourcingBuilder.Services
                .TryAddTransient<IPostgreSqlEventStreamPersistenceConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<PostgreSqlEventStreamPersistenceConfiguration>>().Value);

            eventSourcingBuilder.Services
                .AddTransient<IEventStreamRepository, PostgreSqlEventStreamRepository>()
                .AddTransient<IEventStreamStagingRepository, PostgreSqlEventStreamStagingRepository>();

            return eventSourcingBuilder;
        }
    }
}