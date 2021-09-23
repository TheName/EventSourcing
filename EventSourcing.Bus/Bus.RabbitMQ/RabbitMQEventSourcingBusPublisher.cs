using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using EventSourcing.Bus.RabbitMQ.Configurations;
using EventSourcing.Bus.RabbitMQ.Helpers;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusPublisher : IEventSourcingBusPublisher, IDisposable
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IRabbitMQPublishingChannelConfiguration _eventSourcingPublishingChannelConfiguration;
        private readonly DisposingThreadLocal<IRabbitMQPublishingChannel> _threadLocalPublishingChannel;

        private IRabbitMQPublishingChannel PublishingChannel => _threadLocalPublishingChannel.Value;

        public RabbitMQEventSourcingBusPublisher(
            IRabbitMQConnection connection,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _eventSourcingPublishingChannelConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            _threadLocalPublishingChannel = new DisposingThreadLocal<IRabbitMQPublishingChannel>(CreatePublishingChannel);
        }

        public async Task PublishAsync(EventStreamEntry eventStreamEntry, CancellationToken cancellationToken)
        {
            await PublishingChannel
                .PublishAsync(eventStreamEntry, cancellationToken)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            _threadLocalPublishingChannel.Dispose();
        }

        private IRabbitMQPublishingChannel CreatePublishingChannel()
        {
            return _connection.CreatePublishingChannel(_eventSourcingPublishingChannelConfiguration);
        }
    }
}