using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.Extensions.DependencyInjection.Persistence.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Persistence.PostgreSql.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithPostgreSqlPersistenceAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithPostgreSqlPersistence();
            
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