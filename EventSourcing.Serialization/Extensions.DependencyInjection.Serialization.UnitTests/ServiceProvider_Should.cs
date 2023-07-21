using EventSourcing.Extensions;
using EventSourcing.Extensions.DependencyInjection.Serialization;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Serialization.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithSerializationAndExternalDependencies()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithSerialization();
            
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