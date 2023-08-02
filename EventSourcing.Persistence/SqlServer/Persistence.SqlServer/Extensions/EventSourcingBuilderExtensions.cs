using System;
using System.Data.SqlClient;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.Persistence.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Persistence.SqlServer.Extensions
{
    /// <summary>
    /// The <see cref="IEventSourcingBuilder"/> extensions.
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// Adds SQL Server persistence layer for EventSourcing library.
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
        public static IEventSourcingBuilder WithSqlServerPersistence(this IEventSourcingBuilder eventSourcingBuilder)
        {
            if (eventSourcingBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventSourcingBuilder));
            }

            eventSourcingBuilder.WithPersistence();

            eventSourcingBuilder.Services
                .AddOptions<SqlServerEventStreamPersistenceConfiguration>()
                .BindConfiguration(nameof(SqlServerEventStreamPersistenceConfiguration))
                .Validate(
                    configuration =>
                    {
                        try
                        {
                            _ = new SqlConnectionStringBuilder(configuration.ConnectionString);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    },
                    "Provided connection string is invalid");

            eventSourcingBuilder.Services
                .TryAddTransient<ISqlServerEventStreamPersistenceConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<SqlServerEventStreamPersistenceConfiguration>>().Value);

            eventSourcingBuilder.Services
                .AddTransient<IEventStreamRepository, SqlServerEventStreamRepository>()
                .AddTransient<IEventStreamStagingRepository, SqlServerEventStreamStagingRepository>();

            return eventSourcingBuilder;
        }
    }
}