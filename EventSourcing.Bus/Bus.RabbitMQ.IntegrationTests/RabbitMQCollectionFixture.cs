using System;
using EventSourcing.Extensions.DependencyInjection;
using EventSourcing.Extensions.DependencyInjection.Bus.RabbitMQ;
using EventSourcing.Extensions.DependencyInjection.Serialization.Json;
using EventSourcing.Persistence.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelpers.Logging;
using Xunit.Abstractions;

namespace Bus.RabbitMQ.IntegrationTests
{
    public class RabbitMQCollectionFixture
    {
        private readonly IServiceProvider _serviceProvider;

        private ITestOutputHelper _testOutputHelper;

        private Func<ITestOutputHelper> TestOutputHelperFunc => () => _testOutputHelper;

        public RabbitMQCollectionFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(builder => builder.AddProvider(new XUnitLoggerProvider(TestOutputHelperFunc)))
                .AddSingleton(new Mock<IEventStreamStagingWriter>().Object)
                .AddSingleton(new Mock<IEventStreamWriter>().Object);
            
            serviceCollection
                .AddEventSourcing()
                .WithRabbitMQBus()
                .WithJsonSerialization();

            _serviceProvider = serviceCollection
                .BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateScopes = true,
                    ValidateOnBuild = true
                });
        }

        public RabbitMQCollectionFixture SetTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            return this;
        }

        public T GetService<T>() => 
            _serviceProvider.GetRequiredService<T>();
    }
}