using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Helpers;
using EventSourcing.Bus.RabbitMQ.Providers;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusPublisher : IEventSourcingBusPublisher
    {
        private readonly IRabbitMQChannelProvider _channelProvider;
        private readonly IRabbitMQConfigurationProvider _configurationProvider;
        private readonly ISerializer _serializer;
        private readonly IPublishingChannel _publishingChannel;

        public RabbitMQEventSourcingBusPublisher(
            IRabbitMQChannelProvider channelProvider,
            IRabbitMQConfigurationProvider configurationProvider,
            ISerializer serializer,
            IPublishingChannel publishingChannel)
        {
            _channelProvider = channelProvider ?? throw new ArgumentNullException(nameof(channelProvider));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _publishingChannel = publishingChannel ?? throw new ArgumentNullException(nameof(publishingChannel));
        }

        public async Task PublishAsync(EventStreamEntry eventStreamEntry, CancellationToken cancellationToken)
        {
            var properties = _channelProvider.PublishingChannel.CreateBasicProperties();
            properties.ContentType = "text/plain";
            properties.DeliveryMode = 2;
            properties.Headers = new Dictionary<string, object>
            {
                {"event-type-identifier", eventStreamEntry.EventDescriptor.EventTypeIdentifier.ToString()}
            };

            var serializedEntry = await _serializer.SerializeAsync(eventStreamEntry, cancellationToken).ConfigureAwait(false);
            var serializedEntryBytes = Encoding.UTF8.GetBytes(serializedEntry);

            var taskCompletionSource = _publishingChannel.WaitForAcknowledgment(_channelProvider.PublishingChannel.NextPublishSeqNo, cancellationToken);

            _channelProvider.PublishingChannel.BasicPublish(
                _configurationProvider.ExchangeName,
                _configurationProvider.RoutingKey,
                false,
                properties,
                serializedEntryBytes);

            await taskCompletionSource.Task.ConfigureAwait(false);
        }
    }
}