using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IRabbitMQChannelProvider _channelProvider;
        private readonly IRabbitMQConsumerFactory _consumerFactory;
        private readonly List<string> _consumingTags = new List<string>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public RabbitMQConsumer(
            IRabbitMQChannelProvider channelProvider,
            IRabbitMQConsumerFactory consumerFactory)
        {
            _channelProvider = channelProvider ?? throw new ArgumentNullException(nameof(channelProvider));
            _consumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
        }
        
        public void Consume(string queueName, Func<BasicDeliverEventArgs, CancellationToken, Task> handler)
        {
            var consumingChannel = _channelProvider.ConsumingChannel;
            
            var consumer = _consumerFactory.Create(consumingChannel, handler, _cancellationTokenSource.Token);
            var consumingTag = consumingChannel.BasicConsume(queueName, false, consumer);
            _consumingTags.Add(consumingTag);
        }

        public void StopConsuming()
        {
            _cancellationTokenSource.Cancel();
            foreach (var consumingTag in _consumingTags)
            {
                _channelProvider.ConsumingChannel.BasicCancel(consumingTag);
            }
        }
    }
}