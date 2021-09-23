using System;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Configurations;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Configurations
{
    public class EventSourcingRabbitMQChannelConfiguration_Should
    {
        [Fact]
        public void ThrowArgumentNullException_When_CreatingWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(() => new EventSourcingRabbitMQChannelConfiguration(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNotNullParameters(IEventSourcingConfiguration configuration)
        {
            _ = new EventSourcingRabbitMQChannelConfiguration(configuration);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnNineSecondsTimeSpan_When_GettingPublishingTimeout(EventSourcingRabbitMQChannelConfiguration configuration)
        {
            var result = configuration.PublishingTimeout;

            Assert.Equal(TimeSpan.FromSeconds(9), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectExchangeName_When_GettingExchangeName(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            eventSourcingConfigurationMock
                .SetupGet(sourcingConfiguration => sourcingConfiguration.BoundedContext)
                .Returns(boundedContext);
            
            var result = configuration.ExchangeName;

            var expectedExchangeName = $"EventSourcing.{boundedContext}";
            Assert.Equal(expectedExchangeName, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectQueueName_When_GettingQueueName(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            eventSourcingConfigurationMock
                .SetupGet(sourcingConfiguration => sourcingConfiguration.BoundedContext)
                .Returns(boundedContext);
            
            var result = configuration.QueueName;

            var expectedQueueName = $"EventSourcing.{boundedContext}";
            Assert.Equal(expectedQueueName, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectRoutingKey_When_GettingRoutingKey(
            string boundedContext,
            [Frozen] Mock<IEventSourcingConfiguration> eventSourcingConfigurationMock,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            eventSourcingConfigurationMock
                .SetupGet(sourcingConfiguration => sourcingConfiguration.BoundedContext)
                .Returns(boundedContext);
            
            var result = configuration.RoutingKey;

            var expectedRoutingKey = $"EventSourcing.{boundedContext}";
            Assert.Equal(expectedRoutingKey, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectExchangeType_When_GettingExchangeType(EventSourcingRabbitMQChannelConfiguration configuration)
        {
            var result = configuration.ExchangeType;

            Assert.Equal("direct", result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnCorrectPrefetchCount_When_GettingPrefetchCount(EventSourcingRabbitMQChannelConfiguration configuration)
        {
            var result = configuration.PrefetchCount;

            Assert.Equal(100, result);
        }
    }
}