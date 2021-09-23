using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Channels;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Bus.RabbitMQ.UnitTests.Extensions
{
    internal static class RabbitMQConsumingChannelExtensions
    {
        public static IConnection GetRawConnection(this RabbitMQConsumingChannel consumingChannel)
        {
            return consumingChannel.GetPrivateFieldValue<IConnection>("_rawConnection");
        }

        public static IRabbitMQConsumingChannelConfiguration GetConsumingChannelConfiguration(this RabbitMQConsumingChannel consumingChannel)
        {
            return consumingChannel.GetPrivateFieldValue<IRabbitMQConsumingChannelConfiguration>("_consumingChannelConfiguration");
        }

        public static ISerializer GetSerializer(this RabbitMQConsumingChannel consumingChannel)
        {
            return consumingChannel.GetPrivateFieldValue<ISerializer>("_serializer");
        }

        public static ILogger<RabbitMQConsumingChannel> GetLogger(this RabbitMQConsumingChannel consumingChannel)
        {
            return consumingChannel.GetPrivateFieldValue<ILogger<RabbitMQConsumingChannel>>("_logger");
        }
    }
}