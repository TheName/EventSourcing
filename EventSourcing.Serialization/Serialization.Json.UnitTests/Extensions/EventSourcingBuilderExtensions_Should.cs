using EventSourcing.Extensions;
using EventSourcing.Serialization.Json.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Serialization.Json.UnitTests.Extensions
{
    public class EventSourcingBuilderExtensions_Should
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