using System;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Factories;
using Moq;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Factories
{
    public class RabbitMQConnectionFactory_Should
    {
        [Theory]
        [AutoMoqData]
        internal void ReturnProperlyConfiguredFactory(
            IConnection connection,
            Mock<IConnectionFactory> connectionFactoryMock,
            Uri connectionString,
            string clientProvidedName,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            [Frozen] Mock<IRabbitMQConnectionFactoryProvider> connectionFactoryProviderMock,
            RabbitMQConnectionFactory factory)
        {
            connectionFactoryMock
                .Setup(connectionFactory => connectionFactory.CreateConnection())
                .Returns(connection)
                .Verifiable();
            
            connectionFactoryProviderMock
                .Setup(provider => provider.Get())
                .Returns(connectionFactoryMock.Object);
            
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.ConnectionString)
                .Returns(connectionString.ToString());

            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.ClientProvidedName)
                .Returns(clientProvidedName);

            var result = factory.Create();
            
            Assert.Equal(connection, result);
            connectionFactoryMock.Verify();
            connectionFactoryMock.VerifySet(connectionFactory => connectionFactory.Uri = It.Is<Uri>(uri => uri.ToString() == connectionString.ToString()), Times.Once);
            connectionFactoryMock.VerifySet(connectionFactory => connectionFactory.ClientProvidedName = clientProvidedName, Times.Once);
            connectionFactoryMock.VerifyNoOtherCalls();
        }
    }
}