using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.Extensions.DependencyInjection.Bus;
using EventSourcing.Extensions.DependencyInjection.Bus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Extensions.DependencyInjection.Bus.RabbitMQ.UnitTests
{
    public class ServiceProvider_Should
    {
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithRabbitMQBusAndExternalDependencies_And_UsingDefaults()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithRabbitMQBus();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithRabbitMQBusAndExternalDependencies_And_OptionsWithConsumer()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithRabbitMQBus(new EventSourcingBusBuilderOptions(true));
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithRabbitMQBusAndExternalDependencies_And_OptionsWithoutConsumer()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithRabbitMQBus(new EventSourcingBusBuilderOptions(false));
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithBusUsingRabbitMQAndExternalDependencies_And_UsingDefaults()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEventSourcing()
                .WithBus()
                .UsingRabbitMQ();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithBusUsingRabbitMQAndExternalDependencies_And_OptionsWithConsumer()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithBus(new EventSourcingBusBuilderOptions(true))
                .UsingRabbitMQ();
            
            // External dependencies
            serviceCollection
                .AddEventSourcingSerializationMocks()
                .AddEventSourcingPersistenceMocks()
                .AddMicrosoftLoggerMock();

            serviceCollection.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }
        
        [Fact]
        public void BuildWithoutErrors_When_AddingEventSourcingWithBusUsingRabbitMQAndExternalDependencies_And_OptionsWithoutConsumer()
        {
            var serviceCollection = new ServiceCollection();
                
            serviceCollection
                .AddEventSourcing()
                .WithBus(new EventSourcingBusBuilderOptions(false))
                .UsingRabbitMQ();
            
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