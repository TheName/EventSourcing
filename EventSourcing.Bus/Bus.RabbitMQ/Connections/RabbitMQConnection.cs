using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using EventSourcing.Bus.RabbitMQ.Channels;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Connections
{
    internal class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactoryProvider _factoryProvider;
        private readonly ISerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;

        private readonly Lazy<IConnection> _lazyRawConnection;

        private IConnection RawConnection => _lazyRawConnection.Value;

        public RabbitMQConnection(
            IConnectionFactoryProvider factoryProvider,
            ISerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _factoryProvider = factoryProvider ?? throw new ArgumentNullException(nameof(factoryProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _lazyRawConnection = new Lazy<IConnection>(CreateRawConnection);
        }
        
        public IRabbitMQPublishingChannel CreatePublishingChannel(IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration)
        {
            return new RabbitMQPublishingChannel(RawConnection, publishingChannelConfiguration, _serializer);
        }

        public IRabbitMQConsumingChannel CreateConsumingChannel(IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration)
        {
            return new RabbitMQConsumingChannel(
                RawConnection,
                consumingChannelConfiguration,
                _serializer,
                _loggerFactory.CreateLogger<RabbitMQConsumingChannel>());
        }

        private IConnection CreateRawConnection()
        {
            return _factoryProvider.Get().CreateConnection();
        }
    }
}