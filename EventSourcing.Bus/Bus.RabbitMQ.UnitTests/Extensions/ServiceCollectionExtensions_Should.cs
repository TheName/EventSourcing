using EventSourcing.Bus;
using EventSourcing.Bus.Extensions;
using EventSourcing.Bus.RabbitMQ.Extensions;
using EventSourcing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Extensions;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Extensions
{
    public class ServiceCollectionExtensions_Should
    {
        [Fact]
        public void MakeServiceCollectionEligibleToBuildServiceProviderWithoutErrors_When_AddingEventSourcingWithBusUsingRabbitMQAndExternalDependencies_And_UsingDefaults()
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
        public void MakeServiceCollectionEligibleToBuildServiceProviderWithoutErrors_When_AddingEventSourcingWithBusUsingRabbitMQAndExternalDependencies_And_OptionsWithConsumer()
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
        public void MakeServiceCollectionEligibleToBuildServiceProviderWithoutErrors_When_AddingEventSourcingWithBusUsingRabbitMQAndExternalDependencies_And_OptionsWithoutConsumer()
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