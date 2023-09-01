using EventSourcing.Extensions;
using EventSourcing.ForgettablePayloads.Extensions;
using EventSourcing.ForgettablePayloads.Persistence;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace ForgettablePayloads.UnitTests.Extensions
{
    public class ServiceCollectionExtensions_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndRequiredForgettablePayloadsImplementation()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithForgettablePayloads();

            // Required EventSourcing.ForgettablePayloads implementations
            serviceCollection.AddMock<IForgettablePayloadStorageRepository>();

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
