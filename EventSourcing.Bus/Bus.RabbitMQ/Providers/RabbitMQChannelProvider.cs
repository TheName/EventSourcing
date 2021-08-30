using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Helpers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQChannelProvider : IRabbitMQChannelProvider, IDisposable
    {
        private readonly DisposingThreadLocal<IModel> _threadLocalPublishingChannel;
        private readonly Lazy<IModel> _consumingChannel;

        public IModel PublishingChannel => _threadLocalPublishingChannel.Value;
        public IModel ConsumingChannel => _consumingChannel.Value;

        public RabbitMQChannelProvider(IRabbitMQChannelFactory rabbitMQChannelFactory)
        {
            _threadLocalPublishingChannel = new DisposingThreadLocal<IModel>(rabbitMQChannelFactory.CreatePublishingChannel);
            _consumingChannel = new Lazy<IModel>(rabbitMQChannelFactory.CreateConsumingChannel);
        }

        public void Dispose()
        {
            _threadLocalPublishingChannel?.Dispose();
            if (_consumingChannel.IsValueCreated)
            {
                _consumingChannel.Value.Dispose();
            }
        }
    }
}