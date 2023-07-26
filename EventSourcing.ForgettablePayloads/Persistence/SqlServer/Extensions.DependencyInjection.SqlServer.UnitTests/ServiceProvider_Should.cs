using EventSourcing.Extensions;
using EventSourcing.ForgettablePayloads.Extensions;
using EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.SqlServer.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndForgettablePayloadsUsingSeparateBuilders()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithForgettablePayloads()
                .WithForgettablePayloadsSqlServerPersistence();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddEventSourcingBusMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
    }
}