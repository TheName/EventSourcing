using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQHandlingExceptionProducerFactory : IRabbitMQHandlingExceptionProducerFactory
    {
        private readonly IRabbitMQHandlingExceptionProducingChannelFactory _handlingExceptionProducingChannelFactory;
        private readonly IRabbitMQProducingQueueBindingConfigurationProvider _queueBindingConfigurationProvider;
        private readonly ISerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMQProducerFactory> _logger;

        public RabbitMQHandlingExceptionProducerFactory(
            IRabbitMQHandlingExceptionProducingChannelFactory handlingExceptionProducingChannelFactory,
            IRabbitMQProducingQueueBindingConfigurationProvider queueBindingConfigurationProvider,
            ISerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _handlingExceptionProducingChannelFactory = handlingExceptionProducingChannelFactory ?? throw new ArgumentNullException(nameof(handlingExceptionProducingChannelFactory));
            _queueBindingConfigurationProvider = queueBindingConfigurationProvider ?? throw new ArgumentNullException(nameof(queueBindingConfigurationProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _logger = _loggerFactory.CreateLogger<RabbitMQProducerFactory>();
        }
        
        public async Task<IRabbitMQProducer<T>> CreateAsync<T>(CancellationToken cancellationToken) where T : Exception
        {
            var producingChannel = _handlingExceptionProducingChannelFactory.Create();
            var queueBindingConfiguration = _queueBindingConfigurationProvider.Get(typeof(T));
            await producingChannel.BindQueueAsync(queueBindingConfiguration, cancellationToken)
                .ConfigureAwait(false);

            var producer = new RabbitMQProducer<T>(
                producingChannel,
                _serializer,
                _loggerFactory.CreateLogger<RabbitMQProducer<T>>());
            
            _logger.LogInformation("Created a new RabbitMQ producer: {RabbitMQProducer}", producer);

            return producer;
        }
    }
}