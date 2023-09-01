using System;
using System.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EventSourcing.Persistence.SqlServer.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SQL Server persistence layer for EventSourcing library.
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
        public static IServiceCollection WithSqlServerPersistence(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
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

            serviceCollection
                .TryAddTransient<ISqlServerEventStreamPersistenceConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<SqlServerEventStreamPersistenceConfiguration>>().Value);

            serviceCollection
                .AddTransient<IEventStreamRepository, SqlServerEventStreamRepository>()
                .AddTransient<IEventStreamStagingRepository, SqlServerEventStreamStagingRepository>();

            return serviceCollection;
        }
    }
}
