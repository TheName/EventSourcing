using EventSourcing.Extensions;
using EventSourcing.Extensions.DependencyInjection.Aggregates;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Aggregates.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithAggregatesAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithAggregates();
            
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