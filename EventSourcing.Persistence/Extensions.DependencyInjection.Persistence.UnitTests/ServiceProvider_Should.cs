using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.Extensions.DependencyInjection.Persistence;
using EventSourcing.Persistence;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Persistence.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndRequiredPersistenceImplementation()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithPersistence();
            
            // Required EventSourcing.Persistence implementations
            serviceCollection
                .AddMock<IEventStreamRepository>()
                .AddMock<IEventStreamStagingRepository>();
            
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