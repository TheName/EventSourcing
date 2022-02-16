using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQConsumerFactory : IRabbitMQConsumerFactory
    {
        private readonly IRabbitMQConsumingChannelFactory _consumingChannelFactory;
        private readonly IRabbitMQConsumingQueueBindingConfigurationProvider _queueBindingConfigurationProvider;
        private readonly ISerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMQConsumerFactory> _logger;

        public RabbitMQConsumerFactory(
            IRabbitMQConsumingChannelFactory consumingChannelFactory,
            IRabbitMQConsumingQueueBindingConfigurationProvider queueBindingConfigurationProvider,
            ISerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _consumingChannelFactory = consumingChannelFactory ?? throw new ArgumentNullException(nameof(consumingChannelFactory));
            _queueBindingConfigurationProvider = queueBindingConfigurationProvider ?? throw new ArgumentNullException(nameof(queueBindingConfigurationProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<RabbitMQConsumerFactory>();
        }
        
        public async Task<IRabbitMQConsumer<T>> CreateAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            var consumingChannel = _consumingChannelFactory.Create();
            var queueBindingConfiguration = _queueBindingConfigurationProvider.Get(typeof(T));
            await consumingChannel.BindQueueAsync(queueBindingConfiguration, cancellationToken).ConfigureAwait(false);
            
            var consumer = new RabbitMQConsumer<T>(
                consumingChannel,
                handler,
                _serializer,
                _loggerFactory.CreateLogger<RabbitMQConsumer<T>>());
            
            _logger.LogInformation("Created a new RabbitMQ consumer: {RabbitMQConsumer}", consumer);

            return consumer;
        }
    }
}