using EventSourcing.Extensions;
using EventSourcing.Persistence;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Persistence.IntegrationTests.Base.Extensions
{
    public class ServiceCollectionExtensions_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndRequiredPersistenceImplementation()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddEventSourcing();

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
