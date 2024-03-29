﻿using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DbUp;
using EventSourcing.Bus;
using EventSourcing.Extensions;
using EventSourcing.Extensions.DatabaseMigrations.Persistence.SqlServer.DbUp.Extensions;
using EventSourcing.Persistence.SqlServer;
using EventSourcing.Persistence.SqlServer.Extensions;
using EventSourcing.Serialization.NewtonsoftJson.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence.IntegrationTests.Base;
using TestHelpers.Extensions;
using TestHelpers.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    public class SqlServerCollectionFixture : IAsyncLifetime
    {
        private readonly IServiceProvider _serviceProvider;

        private ITestOutputHelper _testOutputHelper;

        private Func<ITestOutputHelper> TestOutputHelperFunc => () => _testOutputHelper;

        public SqlServerCollectionFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(builder => builder.AddProvider(new XUnitLoggerProvider(TestOutputHelperFunc)))
                .PostConfigure<SqlServerEventStreamPersistenceConfiguration>(persistenceConfiguration =>
                {
                    var sqlConnectionBuilder =
                        new SqlConnectionStringBuilder(persistenceConfiguration.ConnectionString);
                    sqlConnectionBuilder.InitialCatalog = $"{sqlConnectionBuilder.InitialCatalog}_{Guid.NewGuid()}";
                    persistenceConfiguration.ConnectionString = sqlConnectionBuilder.ConnectionString;
                })
                .AddMock<IEventSourcingBusPublisher>()
                .AddMock<IEventSourcingBusHandlingExceptionPublisherConfiguration>()
                .AddMock<IEventSourcingBusHandlingExceptionPublisher>()
                .AddMock<IEventSourcingBusConsumer>()
                .AddTransient<IEventStreamTestReadRepository, SqlServerEventStreamTestReadRepository>()
                .AddTransient<IEventStreamStagingTestRepository, SqlServerEventStreamStagingTestRepository>();

            serviceCollection
                .AddEventSourcing()
                .WithSqlServerPersistence()
                .WithNewtonsoftJsonSerialization();

            _serviceProvider = serviceCollection
                .BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateScopes = true,
                    ValidateOnBuild = true
                });
        }

        public SqlServerCollectionFixture SetTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            return this;
        }

        public T GetService<T>() =>
            _serviceProvider.GetRequiredService<T>();

        public Task InitializeAsync()
        {
            var sqlConnectionString = GetService<ISqlServerEventStreamPersistenceConfiguration>().ConnectionString;
            EnsureDatabase.For.SqlDatabase(sqlConnectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(sqlConnectionString)
                .WithSqlServerScriptsForEventSourcing()
                .LogToAutodetectedLog()
                .Build();

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                throw new Exception("Migrations were not successful!");
            }

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            DropDatabase.For.SqlDatabase(GetService<ISqlServerEventStreamPersistenceConfiguration>().ConnectionString);
            return Task.CompletedTask;
        }
    }
}
