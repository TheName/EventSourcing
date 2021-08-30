using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Serialization.Abstractions;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusConsumer : IEventSourcingBusConsumer
    {
        private readonly IRabbitMQConsumer _consumer;
        private readonly IRabbitMQConfigurationProvider _configurationProvider;
        private readonly ISerializer _serializer;
        private readonly IEventStreamEntryDispatcher _dispatcher;

        public RabbitMQEventSourcingBusConsumer(
            IRabbitMQConsumer consumer,
            IRabbitMQConfigurationProvider configurationProvider,
            ISerializer serializer,
            IEventStreamEntryDispatcher dispatcher)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }
        
        public Task StartConsuming(CancellationToken cancellationToken)
        {
            _consumer.Consume(_configurationProvider.QueueName, ConsumerOnReceived);
            
            return Task.CompletedTask;
        }

        public Task StopConsuming(CancellationToken cancellationToken)
        {
            _consumer.StopConsuming();
            
            return Task.CompletedTask;
        }

        private async Task ConsumerOnReceived(BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            var entry = (EventStreamEntry) _serializer.DeserializeFromUtf8Bytes(args.Body.ToArray(), typeof(EventStreamEntry));
            await _dispatcher.DispatchAsync(entry, cancellationToken).ConfigureAwait(false);
        }
    }
}