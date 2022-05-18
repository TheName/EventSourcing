using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Extensions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace ForgettablePayloads.Extensions.DependencyInjection.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndRequiredForgettablePayloadsImplementation()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithForgettablePayloads();
            
            // Required EventSourcing.ForgettablePayloads implementations
            serviceCollection
                .AddMock<IForgettablePayloadStorageReader>()
                .AddMock<IForgettablePayloadStorageWriter>();
            
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