using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Helpers;
using EventSourcing.Serialization.Abstractions;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Channels
{
    internal class RabbitMQPublishingChannel : IRabbitMQPublishingChannel, IDisposable
    {
        private readonly IConnection _rawConnection;
        private readonly IRabbitMQPublishingChannelConfiguration _publishingChannelConfiguration;
        private readonly ISerializer _serializer;
        private readonly Lazy<IModel> _lazyRawPublishingChannel;
        private readonly Lazy<PublishingAcknowledgmentTracker> _lazyPublishingAcknowledgmentTracker;
        
        private bool _disposed;

        private IModel RawPublishingChannel => _lazyRawPublishingChannel.Value;

        private PublishingAcknowledgmentTracker AcknowledgmentTracker => _lazyPublishingAcknowledgmentTracker.Value;

        public RabbitMQPublishingChannel(
            IConnection rawConnection,
            IRabbitMQPublishingChannelConfiguration publishingChannelConfiguration,
            ISerializer serializer)
        {
            _rawConnection = rawConnection ?? throw new ArgumentNullException(nameof(rawConnection));
            _publishingChannelConfiguration = publishingChannelConfiguration ?? throw new ArgumentNullException(nameof(publishingChannelConfiguration));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _lazyRawPublishingChannel = new Lazy<IModel>(CreatePublishingChannel);
            _lazyPublishingAcknowledgmentTracker = new Lazy<PublishingAcknowledgmentTracker>(() => new PublishingAcknowledgmentTracker());
        }
        
        public async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQPublishingChannel));
            }
            
            var properties = RawPublishingChannel.CreateBasicProperties();
            properties.ContentType = "text/plain";
            properties.DeliveryMode = 2;
            
            var deliveryTagAcknowledgmentTracker = AcknowledgmentTracker.RegisterDeliveryTag(
                RawPublishingChannel.NextPublishSeqNo,
                _publishingChannelConfiguration.PublishingTimeout,
                cancellationToken);
                
            RawPublishingChannel.BasicPublish(
                _publishingChannelConfiguration.ExchangeName,
                _publishingChannelConfiguration.RoutingKey,
                false,
                properties,
                _serializer.SerializeToUtf8Bytes(message));

            try
            {
                var wasPublishedMessageAcknowledged = await deliveryTagAcknowledgmentTracker.ConfigureAwait(false);
                if (!wasPublishedMessageAcknowledged)
                {
                    throw new Exception("Publishing a message to RabbitMQ failed.");
                }
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException($"Publishing a message to RabbitMQ failed; did not get publishing acknowledgment in {_publishingChannelConfiguration.PublishingTimeout}");
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_lazyPublishingAcknowledgmentTracker.IsValueCreated)
            {
                _lazyPublishingAcknowledgmentTracker.Value?.Dispose();
            }

            if (_lazyRawPublishingChannel.IsValueCreated)
            {
                _lazyRawPublishingChannel.Value?.Dispose();
            }
                
            _disposed = true;
        }

        private IModel CreatePublishingChannel()
        {
            var channel = _rawConnection.CreateModel();
            
            channel.ExchangeDeclare(_publishingChannelConfiguration.ExchangeName, _publishingChannelConfiguration.ExchangeType, true);
            channel.QueueDeclare(_publishingChannelConfiguration.QueueName, true, false, false);
            channel.QueueBind(_publishingChannelConfiguration.QueueName, _publishingChannelConfiguration.ExchangeName, _publishingChannelConfiguration.RoutingKey);
            channel.ConfirmSelect();
            channel.BasicAcks += AcknowledgmentTracker.ChannelOnBasicAcks;
            channel.BasicNacks += AcknowledgmentTracker.ChannelOnBasicNacks;

            return channel;
        }
    }
}