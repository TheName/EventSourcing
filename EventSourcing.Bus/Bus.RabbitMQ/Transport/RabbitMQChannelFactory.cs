using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQChannelFactory : 
        IRabbitMQConsumingChannelFactory,
        IRabbitMQProducingChannelFactory,
        IRabbitMQHandlingExceptionProducingChannelFactory,
        IDisposable
    {
        private readonly IRabbitMQConnectionFactory _connectionFactory;
        private readonly IRabbitMQConnectionConfigurationProvider _connectionConfigurationProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMQChannelFactory> _logger;
        private readonly Lazy<IRabbitMQConnection> _lazyConsumerConnection;
        private readonly Lazy<IRabbitMQConnection> _lazyProducerConnection;
        private readonly Lazy<IRabbitMQConnection> _lazyHandlingExceptionProducerConnection;

        private IRabbitMQConnection ConsumerConnection => _lazyConsumerConnection.Value;
        private IRabbitMQConnection ProducerConnection => _lazyProducerConnection.Value;
        private IRabbitMQConnection HandlingExceptionProducerConnection => _lazyHandlingExceptionProducerConnection.Value;

        public RabbitMQChannelFactory(
            IRabbitMQConnectionFactory connectionFactory,
            IRabbitMQConnectionConfigurationProvider connectionConfigurationProvider,
            ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _connectionConfigurationProvider = connectionConfigurationProvider ?? throw new ArgumentNullException(nameof(connectionConfigurationProvider));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<RabbitMQChannelFactory>();

            _lazyConsumerConnection = new Lazy<IRabbitMQConnection>(CreateConsumerConnection, LazyThreadSafetyMode.ExecutionAndPublication);
            _lazyProducerConnection = new Lazy<IRabbitMQConnection>(CreateProducerConnection, LazyThreadSafetyMode.ExecutionAndPublication);
            _lazyHandlingExceptionProducerConnection = new Lazy<IRabbitMQConnection>(CreateHandlingExceptionProducerConnection, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public void Dispose()
        {
            _loggerFactory?.Dispose();
            if (_lazyConsumerConnection.IsValueCreated && _lazyConsumerConnection.Value is IDisposable disposableConsumerConnection)
            {
                disposableConsumerConnection.Dispose();
            }
        }
        
        IRabbitMQConsumingChannel IRabbitMQConsumingChannelFactory.Create()
        {
            var channel = ConsumerConnection.CreateChannel();
            var consumingChannel = new RabbitMQConsumingChannel(channel, _loggerFactory.CreateLogger<RabbitMQConsumingChannel>());
            
            _logger.LogInformation("Created a new RabbitMQ consuming channel: {RabbitMQConsumingChannel}", consumingChannel);

            return consumingChannel;
        }

        IRabbitMQProducingChannel IRabbitMQProducingChannelFactory.Create()
        {
            var channel = ProducerConnection.CreateChannel();
            var producingChannel = new RabbitMQProducingChannel(channel, _loggerFactory.CreateLogger<RabbitMQProducingChannel>());
            
            _logger.LogInformation("Created a new RabbitMQ producing channel: {RabbitMQProducingChannel}", producingChannel);

            return producingChannel;
        }

        IRabbitMQProducingChannel IRabbitMQHandlingExceptionProducingChannelFactory.Create()
        {
            var channel = HandlingExceptionProducerConnection.CreateChannel();
            var producingChannel = new RabbitMQProducingChannel(channel, _loggerFactory.CreateLogger<RabbitMQProducingChannel>());
            
            _logger.LogInformation("Created a new RabbitMQ producing channel: {RabbitMQProducingChannel}", producingChannel);

            return producingChannel;
        }

        private IRabbitMQConnection CreateConsumerConnection()
        {
            var consumerConnectionConfiguration = _connectionConfigurationProvider.ConsumerConnectionConfiguration;
            return _connectionFactory.Create(consumerConnectionConfiguration);
        }

        private IRabbitMQConnection CreateProducerConnection()
        {
            var producerConnectionConfiguration = _connectionConfigurationProvider.ProducerConnectionConfiguration;
            return _connectionFactory.Create(producerConnectionConfiguration);
        }

        private IRabbitMQConnection CreateHandlingExceptionProducerConnection()
        {
            var producerConnectionConfiguration = _connectionConfigurationProvider.HandlingExceptionProducerConnectionConfiguration;
            return _connectionFactory.Create(producerConnectionConfiguration);
        }
    }
}