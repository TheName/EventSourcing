using System;
using EventSourcing.Bus.RabbitMQ.Helpers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQChannelProvider : IRabbitMQChannelProvider
    {
        private readonly IRabbitMQConnectionProvider _connectionProvider;
        private readonly IRabbitMQConfigurationProvider _configurationProvider;
        private readonly IPublishingChannel _publishingChannel;
        private readonly DisposableThreadLocal<IModel> _threadLocalPublishingChannel;

        public IModel PublishingChannel => _threadLocalPublishingChannel.Value;

        public RabbitMQChannelProvider(
            IRabbitMQConnectionProvider connectionProvider,
            IRabbitMQConfigurationProvider configurationProvider,
            IPublishingChannel publishingChannel)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _publishingChannel = publishingChannel ?? throw new ArgumentNullException(nameof(publishingChannel));

            _threadLocalPublishingChannel = new DisposableThreadLocal<IModel>(CreatePublishingChannel, true);
        }

        private IModel CreatePublishingChannel()
        {
            var channel = _connectionProvider.Connection.CreateModel();
            
            channel.ExchangeDeclare(_configurationProvider.ExchangeName, "direct", true);
            channel.QueueDeclare(_configurationProvider.QueueName, true, false, false);
            channel.QueueBind(_configurationProvider.QueueName, _configurationProvider.ExchangeName, _configurationProvider.RoutingKey);
            channel.ConfirmSelect();
            channel.BasicAcks += (sender, args) => _publishingChannel.Ack(args.DeliveryTag, args.Multiple);
            channel.BasicNacks += (sender, args) => _publishingChannel.Nack(args.DeliveryTag, args.Multiple);

            return channel;
        }
        
        public void Disconnect()
        {
            if (_threadLocalPublishingChannel.IsValueCreated)
            {
                foreach (var lazyChannelValue in _threadLocalPublishingChannel.Values)
                {
                    lazyChannelValue?.Dispose();
                }
            }
        }
    }
}