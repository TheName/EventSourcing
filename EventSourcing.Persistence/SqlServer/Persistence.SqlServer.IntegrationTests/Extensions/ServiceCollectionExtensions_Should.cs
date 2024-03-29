﻿using EventSourcing.Extensions;
using EventSourcing.Persistence.SqlServer.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Persistence.SqlServer.IntegrationTests.Extensions
{
    public class ServiceCollectionExtensions_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithSqlServerPersistenceAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithSqlServerPersistence();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingBusMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
    }
}
