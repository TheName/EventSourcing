using System;
using System.Reflection;
using EventSourcing.Persistence.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventSourcing.Persistence.PostgreSql.Extensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds PostgreSQL persistence layer for EventSourcing library.
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
        public static IServiceCollection WithPostgreSqlPersistence(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.WithPersistence();

            serviceCollection
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

            serviceCollection
                .TryAddTransient<IPostgreSqlEventStreamPersistenceConfiguration>(provider =>
                    provider.GetRequiredService<IOptions<PostgreSqlEventStreamPersistenceConfiguration>>().Value);

            serviceCollection
                .AddTransient<IEventStreamRepository, PostgreSqlEventStreamRepository>()
                .AddTransient<IEventStreamStagingRepository, PostgreSqlEventStreamStagingRepository>();

            return serviceCollection;
        }
    }
}
