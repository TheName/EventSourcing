using System.Linq;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Providers;
using Moq;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Providers
{
    public class RabbitMQConnectionProvider_Should
    {
        [Theory]
        [AutoMoqData]
        internal void DoNothing_When_ProviderDisposedWithoutGettingConnection(Mock<IRabbitMQConnectionFactory> connectionFactoryMock)
        {
            var provider = new RabbitMQConnectionProvider(connectionFactoryMock.Object);
            provider.Dispose();
            
            connectionFactoryMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnConnectionFromFactory_When_GettingConnection(
            IConnection connection,
            Mock<IRabbitMQConnectionFactory> connectionFactoryMock)
        {
            var provider = new RabbitMQConnectionProvider(connectionFactoryMock.Object);
            connectionFactoryMock
                .Setup(factory => factory.Create())
                .Returns(connection);

            var result = provider.Connection;
            
            Assert.Equal(result, connection);
        }
        
        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        internal void CreateConnectionFromFactoryOnlyOnce_When_GettingConnectionMultipleTimes(
            int numberOfTimesToGetConnection,
            IConnection connection,
            Mock<IRabbitMQConnectionFactory> connectionFactoryMock)
        {
            var provider = new RabbitMQConnectionProvider(connectionFactoryMock.Object);
            connectionFactoryMock
                .Setup(factory => factory.Create())
                .Returns(connection);

            foreach (var i in Enumerable.Range(0, numberOfTimesToGetConnection))
            {
                _ = provider.Connection;
            }
            
            connectionFactoryMock.Verify(factory => factory.Create(), Times.Once);
        }
        
        [Theory]
        [AutoMoqData]
        internal void DisposeCreatedConnectionFromFactory_When_DisposingProvider(
            Mock<IRabbitMQConnectionFactory> connectionFactoryMock)
        {
            var provider = new RabbitMQConnectionProvider(connectionFactoryMock.Object);
            var connectionMock = new Mock<IConnection>();
            connectionFactoryMock
                .Setup(factory => factory.Create())
                .Returns(connectionMock.Object);

            _ = provider.Connection;
            provider.Dispose();
            
            connectionMock.Verify(connection => connection.Dispose(), Times.Once);
        }
    }
}