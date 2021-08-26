﻿using System;
using System.Data.SqlClient;
using EventSourcing.Persistence;
using EventSourcing.Persistence.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Extensions.DependencyInjection.Persistence.SqlServer
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
                .AddTransient<IEventStreamWriteRepository, SqlServerEventStreamWriteRepository>()
                .AddTransient<IEventStreamStagingWriteRepository, SqlServerEventStreamStagingWriteRepository>();

            return eventSourcingBuilder;
        }
    }
}