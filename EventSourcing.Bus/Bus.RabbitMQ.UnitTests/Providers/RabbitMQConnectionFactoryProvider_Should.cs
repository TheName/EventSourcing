using EventSourcing.Bus.RabbitMQ.Providers;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Providers
{
    public class RabbitMQConnectionFactoryProvider_Should
    {
        [Theory]
        [AutoMoqData]
        internal void ReturnConnectionFactory(RabbitMQConnectionFactoryProvider provider)
        {
            var result = provider.Get();

            Assert.NotNull(result);
            var connectionFactory = Assert.IsType<ConnectionFactory>(result);
            Assert.True(connectionFactory.DispatchConsumersAsync);
            Assert.True(connectionFactory.AutomaticRecoveryEnabled);
            Assert.True(connectionFactory.TopologyRecoveryEnabled);
        }
    }
}