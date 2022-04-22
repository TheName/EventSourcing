using EventSourcing.Bus.Abstractions;
using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.Extensions.DependencyInjection.Bus;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Bus.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithBusAndExternalDependenciesAndRequiredBusImplementation_And_UsingDefaults()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithBus();
            
            // Required EventSourcing.Bus implementations
            serviceCollection
                .AddMock<IEventSourcingBusConsumer>()
                .AddMock<IEventSourcingBusHandlingExceptionPublisher>()
                .AddMock<IEventSourcingBusPublisher>();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithBusAndExternalDependenciesAndBusImplementation_And_OptionsWithConsumer()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithBus(new EventSourcingBusBuilderOptions(true));
            
            // Required EventSourcing.Bus implementations
            serviceCollection
                .AddMock<IEventSourcingBusConsumer>()
                .AddMock<IEventSourcingBusHandlingExceptionPublisher>()
                .AddMock<IEventSourcingBusPublisher>();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithBusAndExternalDependenciesAndBusImplementation_And_OptionsWithoutConsumer()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithBus(new EventSourcingBusBuilderOptions(false));
            
            // Required EventSourcing.Bus implementations
            serviceCollection
                .AddMock<IEventSourcingBusHandlingExceptionPublisher>()
                .AddMock<IEventSourcingBusPublisher>();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
    }
}