using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IRabbitMQChannelProvider _channelProvider;
        private readonly ISerializer _serializer;
        private readonly IRabbitMQPublishAcknowledgmentTracker _publishAcknowledgmentTracker;

        public RabbitMQPublisher(
            IRabbitMQChannelProvider channelProvider,
            ISerializer serializer,
            IRabbitMQPublishAcknowledgmentTracker publishAcknowledgmentTracker)
        {
            _channelProvider = channelProvider ?? throw new ArgumentNullException(nameof(channelProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _publishAcknowledgmentTracker = publishAcknowledgmentTracker ?? throw new ArgumentNullException(nameof(publishAcknowledgmentTracker));
        }
        
        public async Task PublishAsync<T>(
            T message,
            string exchangeName,
            string routingKey,
            IReadOnlyDictionary<string, string> headers, 
            CancellationToken cancellationToken)
        {
            var properties = _channelProvider.PublishingChannel.CreateBasicProperties();
            properties.ContentType = "text/plain";
            properties.DeliveryMode = 2;
            properties.Headers = headers.ToDictionary(pair => pair.Key, pair => (object) pair.Value);

            var serializedEntryBytes = _serializer.SerializeToUtf8Bytes(message);

            var taskCompletionSource = _publishAcknowledgmentTracker.WaitForAcknowledgment(_channelProvider.PublishingChannel.NextPublishSeqNo, cancellationToken);

            _channelProvider.PublishingChannel.BasicPublish(
                exchangeName,
                routingKey,
                false,
                properties,
                serializedEntryBytes);

            try
            {
                var publishingResult = await taskCompletionSource.Task.ConfigureAwait(false);
                if (!publishingResult)
                {
                    throw new Exception("Failed to publish message.");
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Did not get publishing acknowledgment in time.", exception);
            }
        }
    }
}