using EventSourcing.Extensions;
using EventSourcing.Extensions.DependencyInjection.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Serialization.Json.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithJsonSerializationAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithJsonSerialization();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingPersistenceMocks()
                .AddEventSourcingBusMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
    }
}