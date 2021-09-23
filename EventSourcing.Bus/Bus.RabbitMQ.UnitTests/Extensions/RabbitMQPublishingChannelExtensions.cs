using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Channels;
using EventSourcing.Serialization.Abstractions;
using RabbitMQ.Client;

namespace Bus.RabbitMQ.UnitTests.Extensions
{
    internal static class RabbitMQPublishingChannelExtensions
    {
        public static IConnection GetRawConnection(this RabbitMQPublishingChannel publishingChannel)
        {
            return publishingChannel.GetPrivateFieldValue<IConnection>("_rawConnection");
        }

        public static IRabbitMQPublishingChannelConfiguration GetPublishingChannelConfiguration(this RabbitMQPublishingChannel publishingChannel)
        {
            return publishingChannel.GetPrivateFieldValue<IRabbitMQPublishingChannelConfiguration>("_publishingChannelConfiguration");
        }

        public static ISerializer GetSerializer(this RabbitMQPublishingChannel publishingChannel)
        {
            return publishingChannel.GetPrivateFieldValue<ISerializer>("_serializer");
        }
    }
}