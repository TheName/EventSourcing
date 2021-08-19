using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Providers;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class EventSourcingBusInitializer : IEventSourcingBusInitializer
    {
        private readonly IRabbitMQChannelProvider _channelProvider;
        private readonly IRabbitMQConnectionProvider _connectionProvider;

        public EventSourcingBusInitializer(
            IRabbitMQChannelProvider channelProvider,
            IRabbitMQConnectionProvider connectionProvider)
        {
            _channelProvider = channelProvider ?? throw new ArgumentNullException(nameof(channelProvider));
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }
        
        public Task InitializeConnectionAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task DisposeConnectionAsync(CancellationToken cancellationToken)
        {
            _channelProvider.Disconnect();
            _connectionProvider.Disconnect();
            
            return Task.CompletedTask;
        }
    }
}