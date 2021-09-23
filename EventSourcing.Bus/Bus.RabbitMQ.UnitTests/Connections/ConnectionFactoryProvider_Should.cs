using System;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Connections;
using Moq;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Connections
{
    public class ConnectionFactoryProvider_Should
    {
        [Fact]
        internal void Throw_ArgumentNullException_When_CreatingWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(() => new ConnectionFactoryProvider(null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_CreatingWithNotNullParameters(IRabbitMQConfiguration configuration)
        {
            _ = new ConnectionFactoryProvider(configuration);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnNewInstanceOfConfiguredConnectionFactory_When_GettingConnectionFactory(
            Uri connectionString,
            [Frozen] Mock<IRabbitMQConfiguration> configurationMock,
            ConnectionFactoryProvider factoryProvider)
        {
            var ampqConnectionString = connectionString.AbsoluteUri.Replace("http", "amqp");
            configurationMock
                .SetupGet(configuration => configuration.ConnectionString)
                .Returns(ampqConnectionString);
            
            var result = factoryProvider.Get();
            
            var connectionFactory = Assert.IsType<ConnectionFactory>(result);
            Assert.NotNull(connectionFactory);
            Assert.True(connectionFactory.DispatchConsumersAsync);
            Assert.Equal(50, connectionFactory.ConsumerDispatchConcurrency);
            Assert.Equal(ampqConnectionString, connectionFactory.Uri.AbsoluteUri);
        }
    }
}