using System;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Providers;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Providers
{
    public class RabbitMQConfigurationProvider_Should
    {
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectExchangeName(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            RabbitMQConfigurationProvider provider)
        {
            eventSourcingConfigurationMock
                .SetupGet(configuration => configuration.BoundedContext)
                .Returns(boundedContext);

            var result = provider.ExchangeName;

            var expectedValue = $"{boundedContext}.EventSourcing";
            Assert.Equal(expectedValue, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectExchangeType(RabbitMQConfigurationProvider provider)
        {
            var result = provider.ExchangeType;

            const string expectedValue = "direct";
            Assert.Equal(expectedValue, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectQueueName(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            RabbitMQConfigurationProvider provider)
        {
            eventSourcingConfigurationMock
                .SetupGet(configuration => configuration.BoundedContext)
                .Returns(boundedContext);

            var result = provider.QueueName;

            var expectedValue = $"{boundedContext}.EventSourcing";
            Assert.Equal(expectedValue, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectRoutingKey(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            RabbitMQConfigurationProvider provider)
        {
            eventSourcingConfigurationMock
                .SetupGet(configuration => configuration.BoundedContext)
                .Returns(boundedContext);

            var result = provider.RoutingKey;

            var expectedValue = $"{boundedContext}.EventSourcing";
            Assert.Equal(expectedValue, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectClientProvidedName(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            RabbitMQConfigurationProvider provider)
        {
            eventSourcingConfigurationMock
                .SetupGet(configuration => configuration.BoundedContext)
                .Returns(boundedContext);

            var result = provider.ClientProvidedName;

            var expectedValue = $"bounded-context: {boundedContext}";
            Assert.Equal(expectedValue, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectConnectionString(
            string connectionString,
            [Frozen] Mock<IRabbitMQConfiguration> rabbitMQConfigurationMock,
            RabbitMQConfigurationProvider provider)
        {
            rabbitMQConfigurationMock
                .SetupGet(configuration => configuration.ConnectionString)
                .Returns(connectionString);

            var result = provider.ConnectionString;

            Assert.Equal(connectionString, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectPublishingTimeout(RabbitMQConfigurationProvider provider)
        {
            var result = provider.PublishingTimeout;

            var expectedValue = TimeSpan.FromSeconds(9);
            Assert.Equal(expectedValue, result);
        }
    }
}