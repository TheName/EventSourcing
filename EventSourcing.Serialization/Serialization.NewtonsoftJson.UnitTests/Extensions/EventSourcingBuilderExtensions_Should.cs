using EventSourcing.Extensions;
using EventSourcing.Serialization.NewtonsoftJson.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Serialization.NewtonsoftJson.UnitTests.Extensions
{
    public class EventSourcingBuilderExtensions_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithNewtonsoftJsonSerializationAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithNewtonsoftJsonSerialization();
            
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