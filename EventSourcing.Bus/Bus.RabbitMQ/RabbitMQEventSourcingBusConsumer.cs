using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using EventSourcing.Bus.RabbitMQ.Configurations;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusConsumer : IEventSourcingBusConsumer
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IRabbitMQConsumingChannelConfiguration _eventSourcingConsumingChannelConfiguration;
        private readonly Lazy<IRabbitMQConsumingChannel> _lazyConsumingChannel;

        private bool _started;

        private IRabbitMQConsumingChannel ConsumingChannel => _lazyConsumingChannel.Value;

        public RabbitMQEventSourcingBusConsumer(
            IRabbitMQConnection connection,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _eventSourcingConsumingChannelConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            _lazyConsumingChannel = new Lazy<IRabbitMQConsumingChannel>(CreateConsumingChannel);
        }
        
        public Task StartConsuming(Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc, CancellationToken cancellationToken)
        {
            if (_started)
            {
                throw new InvalidOperationException("Consuming has already been started.");
            }
            
            ConsumingChannel.AddConsumer(consumingTaskFunc, cancellationToken);
            _started = true;
            
            return Task.CompletedTask;
        }

        public Task StopConsuming(CancellationToken cancellationToken)
        {
            if (_lazyConsumingChannel.IsValueCreated && _lazyConsumingChannel.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return Task.CompletedTask;
        }

        private IRabbitMQConsumingChannel CreateConsumingChannel()
        {
            return _connection.CreateConsumingChannel(_eventSourcingConsumingChannelConfiguration);
        }
    }
}