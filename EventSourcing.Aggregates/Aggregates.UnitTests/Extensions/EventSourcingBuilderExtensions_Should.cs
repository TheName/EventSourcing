using EventSourcing.Aggregates.Extensions;
using EventSourcing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Aggregates.UnitTests.Extensions
{
    public class EventSourcingBuilderExtensions_Should
    {
        [Fact]
        public void MakeServiceCollectionEligibleToBuildServiceProviderWithoutErrors_When_AddingEventSourcingWithAggregatesAndExternalDependencies()
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