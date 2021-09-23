using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Channels
{
    internal class RabbitMQConsumingChannel : IRabbitMQConsumingChannel, IDisposable
    {
        private const string BasicDeliverEventArgsInfoMessageTemplate = "Consumer tag: \"{consumerTag}\", Delivery tag: \"{deliveryTag}\", Redelivered: \"{redelivered}\", Exchange: \"{exchange}\", Routing key: \"{routingKey}\", Properties.AppId: \"{properties.appId}\", Properties.ClusterId: \"{properties.clusterId}\", Properties.ContentEncoding: \"{properties.contentEncoding}\", Properties.ContentType: \"{properties.contentType}\", Properties.CorrelationId: \"{properties.correlationId}\", Properties.DeliveryMode: \"{properties.deliveryMode}\", Properties.Expiration: \"{properties.expiration}\", Properties.Headers: \"{properties.headers}\", Properties.MessageId: \"{properties.messageId}\", Properties.Persistent: \"{properties.persistent}\", Properties.Priority: \"{properties.priority}\", Properties.ReplyTo: \"{properties.replyTo}\", Properties.Timestamp: \"{properties.timestamp}\", Properties.Type: \"{properties.type}\", Properties.UserId: \"{properties.userId}\", Body (bytes): \"{body}\"";
        
        private readonly IConnection _rawConnection;
        private readonly IRabbitMQConsumingChannelConfiguration _consumingChannelConfiguration;
        private readonly ISerializer _serializer;
        private readonly ILogger<RabbitMQConsumingChannel> _logger;
        private readonly ConcurrentQueue<string> _consumingTags = new ConcurrentQueue<string>();

        private readonly Lazy<IModel> _lazyRawConsumingChannel;
        
        private IModel RawConsumingChannel => _lazyRawConsumingChannel.Value;

        public RabbitMQConsumingChannel(
            IConnection rawConnection,
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            ISerializer serializer,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            _rawConnection = rawConnection ?? throw new ArgumentNullException(nameof(rawConnection));
            _consumingChannelConfiguration = consumingChannelConfiguration ?? throw new ArgumentNullException(nameof(consumingChannelConfiguration));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _lazyRawConsumingChannel = new Lazy<IModel>(CreateConsumingChannel);
        }

        public void AddConsumer<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(RawConsumingChannel);
            consumer.Received += (sender, args) => ConsumerOnReceived(args, handler, cancellationToken); 

            var consumingTag = RawConsumingChannel.BasicConsume(_consumingChannelConfiguration.QueueName, false, consumer);
            _consumingTags.Enqueue(consumingTag);
        }

        public void Dispose()
        {
            foreach (var consumingTag in _consumingTags)
            {
                RawConsumingChannel.BasicCancel(consumingTag);
            }
            
            RawConsumingChannel?.Dispose();
        }

        private IModel CreateConsumingChannel()
        {
            var channel = _rawConnection.CreateModel();
            
            channel.ExchangeDeclare(_consumingChannelConfiguration.ExchangeName, _consumingChannelConfiguration.ExchangeType, true);
            channel.QueueDeclare(_consumingChannelConfiguration.QueueName, true, false, false);
            channel.QueueBind(_consumingChannelConfiguration.QueueName, _consumingChannelConfiguration.ExchangeName, _consumingChannelConfiguration.RoutingKey);
            channel.BasicQos(0, _consumingChannelConfiguration.PrefetchCount, false);

            return channel;
        }

        private async Task ConsumerOnReceived<T>(
            BasicDeliverEventArgs args,
            Func<T, CancellationToken, Task> handler,
            CancellationToken cancellationToken)
        {
            T message;

            try
            {
                message = (T) _serializer.DeserializeFromUtf8Bytes(args.Body.ToArray(), typeof(T));
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    $"Consuming message failed - deserialization to type \"{{type}}\" failed; not acknowledging. {BasicDeliverEventArgsInfoMessageTemplate}",
                    new object[] {typeof(T)}.Concat(GetBasicDeliverEventArgsInfoMessageParameters(args)).ToArray());

                return;
            }

            try
            {
                await handler(message, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException taskCanceledException)
            {
                _logger.LogError(
                    taskCanceledException,
                    $"Consuming message failed - handling message \"{{message}}\" of type \"{{type}}\" was cancelled; not acknowledging. {BasicDeliverEventArgsInfoMessageTemplate}",
                    new object[] {message, typeof(T)}.Concat(GetBasicDeliverEventArgsInfoMessageParameters(args))
                        .ToArray());

                return;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    $"Consuming message failed - handling message \"{{message}}\" of type \"{{type}}\" has thrown an exception; not acknowledging. {BasicDeliverEventArgsInfoMessageTemplate}",
                    new object[] {message, typeof(T)}.Concat(GetBasicDeliverEventArgsInfoMessageParameters(args))
                        .ToArray());

                return;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    $"Consuming message failed - handling message \"{{message}}\" of type \"{{type}}\" was cancelled; not acknowledging. {BasicDeliverEventArgsInfoMessageTemplate}",
                    new object[] {message, typeof(T)}.Concat(GetBasicDeliverEventArgsInfoMessageParameters(args))
                        .ToArray());

                return;
            }

            try
            {
                RawConsumingChannel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    $"Consuming message failed - handling message \"{{message}}\" of type \"{{type}}\" was successful but acknowledging failed. {BasicDeliverEventArgsInfoMessageTemplate}",
                    new object[] {message, typeof(T)}.Concat(GetBasicDeliverEventArgsInfoMessageParameters(args))
                        .ToArray());

                return;
            }
            
            _logger.LogDebug(
                $"Consuming message succeeded - handling message \"{{message}}\" of type \"{{type}}\" was successful; acknowledged. {BasicDeliverEventArgsInfoMessageTemplate}",
                new object[] {message, typeof(T)}.Concat(GetBasicDeliverEventArgsInfoMessageParameters(args))
                    .ToArray());
        }

        private static object[] GetBasicDeliverEventArgsInfoMessageParameters(BasicDeliverEventArgs args)
        {
            return new object[]
            {
                args?.ConsumerTag,
                args?.DeliveryTag,
                args?.Redelivered,
                args?.Exchange,
                args?.RoutingKey,
                args?.BasicProperties?.AppId,
                args?.BasicProperties?.ClusterId,
                args?.BasicProperties?.ContentEncoding,
                args?.BasicProperties?.ContentType,
                args?.BasicProperties?.CorrelationId,
                args?.BasicProperties?.DeliveryMode,
                args?.BasicProperties?.Expiration,
                string.Join(", ", args?.BasicProperties?.Headers?.Select(pair => $"{{key: {pair.Key}, value: {pair.Value}}}") ?? new List<string>()),
                args?.BasicProperties?.MessageId,
                args?.BasicProperties?.Persistent,
                args?.BasicProperties?.Priority,
                args?.BasicProperties?.ReplyTo,
                args?.BasicProperties?.Timestamp,
                args?.BasicProperties?.Type,
                args?.BasicProperties?.UserId,
                Encoding.UTF8.GetString(args?.Body.ToArray() ?? new byte[0])
            };
        }
    }
}