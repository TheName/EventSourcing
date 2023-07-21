using EventSourcing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace EventSourcing.UnitTests.Extensions
{
    public class ServiceCollectionExtensions_Should
    {
        [Fact]
        public void MakeServiceCollectionEligibleToBuildServiceProviderWithoutErrors_When_AddingEventSourcingAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection.AddEventSourcing();
            
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