using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using EventSourcing.Extensions;
using EventSourcing.ForgettablePayloads.Extensions;
using EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.PostgreSql.DbUp.Extensions;
using EventSourcing.ForgettablePayloads.Persistence.PostgreSql;
using EventSourcing.ForgettablePayloads.Persistence.PostgreSql.Extensions;
using EventSourcing.Serialization.Json.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestHelpers.Extensions;
using TestHelpers.Logging;
using Xunit;
using Xunit.Abstractions;

namespace ForgettablePayloads.Persistence.PostgreSql.IntegrationTests
{
    public class PostgreSqlCollectionFixture : IAsyncLifetime
    {
        private readonly IServiceProvider _serviceProvider;

        private ITestOutputHelper _testOutputHelper;

        private Func<ITestOutputHelper> TestOutputHelperFunc => () => _testOutputHelper;

        public PostgreSqlCollectionFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(builder => builder.AddProvider(new XUnitLoggerProvider(TestOutputHelperFunc)))
                .PostConfigure<PostgreSqlEventStreamForgettablePayloadPersistenceConfiguration>(
                    persistenceConfiguration =>
                    {
                        var sqlConnectionBuilder =
                            new NpgsqlConnectionStringBuilder(persistenceConfiguration.ConnectionString);
                        sqlConnectionBuilder.Database = $"{sqlConnectionBuilder.Database}_{Guid.NewGuid()}";
                        persistenceConfiguration.ConnectionString = sqlConnectionBuilder.ConnectionString;
                    });

            serviceCollection
                .AddEventSourcing()
                .WithForgettablePayloads()
                .WithForgettablePayloadsPostgreSqlPersistence()
                .WithJsonSerialization();

            serviceCollection
                .AddEventSourcingPersistenceMocks()
                .AddEventSourcingBusMocks();

            _serviceProvider = serviceCollection
                .BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateScopes = true,
                    ValidateOnBuild = true
                });
        }

        public PostgreSqlCollectionFixture SetTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            return this;
        }

        public T GetService<T>() =>
            _serviceProvider.GetRequiredService<T>();

        public Task InitializeAsync()
        {
            var sqlConnectionString = GetService<IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration>().ConnectionString;
            EnsureDatabase.For.PostgresqlDatabase(sqlConnectionString);

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(sqlConnectionString)
                .WithPostgreSqlScriptsForEventSourcingForgettablePayloads()
                .LogToAutodetectedLog()
                .Build();

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                throw new Exception("Migrations were not successful!");
            }

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            var sqlConnectionBuilder = new NpgsqlConnectionStringBuilder(GetService<IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration>().ConnectionString);
            var databaseToDelete = sqlConnectionBuilder.Database;
            sqlConnectionBuilder.Database = null;

            using (var connection = new NpgsqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{databaseToDelete}'; DROP DATABASE \"{databaseToDelete}\"";
                    command.CommandType = CommandType.Text;
                    await connection.OpenAsync(CancellationToken.None);
                    await command.ExecuteNonQueryAsync(CancellationToken.None);
                }
            }
        }
    }
}
