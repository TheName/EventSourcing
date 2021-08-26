using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Bus.RabbitMQ.Helpers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQChannelProvider : IRabbitMQChannelProvider, IDisposable
    {
        private readonly DisposingThreadLocal<IModel> _threadLocalPublishingChannel;

        public IModel PublishingChannel => _threadLocalPublishingChannel.Value;

        public RabbitMQChannelProvider(IRabbitMQChannelFactory rabbitMQChannelFactory)
        {
            _threadLocalPublishingChannel = new DisposingThreadLocal<IModel>(rabbitMQChannelFactory.Create);
        }

        public void Dispose()
        {
            _threadLocalPublishingChannel?.Dispose();
        }
    }
}