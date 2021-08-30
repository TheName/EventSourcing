using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Factories
{
    internal class RabbitMQChannelFactory : IRabbitMQChannelFactory
    {
        private readonly IRabbitMQConnectionProvider _rabbitMQConnectionProvider;
        private readonly IRabbitMQConfigurationProvider _rabbitMQConfigurationProvider;
        private readonly IRabbitMQPublishAcknowledgmentTracker _rabbitMQPublishingChannel;

        public RabbitMQChannelFactory(
            IRabbitMQConnectionProvider rabbitMQConnectionProvider,
            IRabbitMQConfigurationProvider rabbitMQConfigurationProvider,
            IRabbitMQPublishAcknowledgmentTracker rabbitMQPublishingChannel)
        {
            _rabbitMQConnectionProvider = rabbitMQConnectionProvider ?? throw new ArgumentNullException(nameof(rabbitMQConnectionProvider));
            _rabbitMQConfigurationProvider = rabbitMQConfigurationProvider ?? throw new ArgumentNullException(nameof(rabbitMQConfigurationProvider));
            _rabbitMQPublishingChannel = rabbitMQPublishingChannel ?? throw new ArgumentNullException(nameof(rabbitMQPublishingChannel));
        }
        
        public IModel CreatePublishingChannel()
        {
            var channel = _rabbitMQConnectionProvider.Connection.CreateModel();
            
            channel.ExchangeDeclare(_rabbitMQConfigurationProvider.ExchangeName, _rabbitMQConfigurationProvider.ExchangeType, true);
            channel.QueueDeclare(_rabbitMQConfigurationProvider.QueueName, true, false, false);
            channel.QueueBind(_rabbitMQConfigurationProvider.QueueName, _rabbitMQConfigurationProvider.ExchangeName, _rabbitMQConfigurationProvider.RoutingKey);
            channel.ConfirmSelect();
            channel.BasicAcks += _rabbitMQPublishingChannel.AckEventHandler;
            channel.BasicNacks += _rabbitMQPublishingChannel.NackEventHandler;

            return channel;
        }

        public IModel CreateConsumingChannel()
        {
            var channel = _rabbitMQConnectionProvider.Connection.CreateModel();
            
            channel.ExchangeDeclare(_rabbitMQConfigurationProvider.ExchangeName, _rabbitMQConfigurationProvider.ExchangeType, true);
            channel.QueueDeclare(_rabbitMQConfigurationProvider.QueueName, true, false, false);
            channel.QueueBind(_rabbitMQConfigurationProvider.QueueName, _rabbitMQConfigurationProvider.ExchangeName, _rabbitMQConfigurationProvider.RoutingKey);

            return channel;
        }
    }
}