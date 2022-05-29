using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Extensions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence;
using EventSourcing.ForgettablePayloads.Persistence;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace ForgettablePayloads.DependencyInjection.Persistence.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndRequiredForgettablePayloadsPersistenceImplementation_Using_SeparateExtensionMethods()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithForgettablePayloads()
                .WithForgettablePayloadsPersistence();
            
            // Required EventSourcing.ForgettablePayloads.Persistence implementations
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
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingAndExternalDependenciesAndRequiredForgettablePayloadsPersistenceImplementation()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithForgettablePayloadsAndPersistence();
            
            // Required EventSourcing.ForgettablePayloads.Persistence implementations
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