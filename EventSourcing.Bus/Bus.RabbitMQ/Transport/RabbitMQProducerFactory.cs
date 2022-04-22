using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQProducerFactory : IRabbitMQProducerFactory
    {
        private readonly IRabbitMQProducingChannelFactory _producingChannelFactory;
        private readonly IRabbitMQProducingQueueBindingConfigurationProvider _queueBindingConfigurationProvider;
        private readonly ISerializerProvider _serializerProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMQProducerFactory> _logger;

        public RabbitMQProducerFactory(
            IRabbitMQProducingChannelFactory producingChannelFactory,
            IRabbitMQProducingQueueBindingConfigurationProvider queueBindingConfigurationProvider,
            ISerializerProvider serializerProvider,
            ILoggerFactory loggerFactory)
        {
            _producingChannelFactory = producingChannelFactory ?? throw new ArgumentNullException(nameof(producingChannelFactory));
            _queueBindingConfigurationProvider = queueBindingConfigurationProvider ?? throw new ArgumentNullException(nameof(queueBindingConfigurationProvider));
            _serializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _logger = _loggerFactory.CreateLogger<RabbitMQProducerFactory>();
        }
        
        public async Task<IRabbitMQProducer<T>> CreateAsync<T>(CancellationToken cancellationToken)
        {
            var producingChannel = _producingChannelFactory.Create();
            var queueBindingConfiguration = _queueBindingConfigurationProvider.Get(typeof(T));
            await producingChannel.BindQueueAsync(queueBindingConfiguration, cancellationToken)
                .ConfigureAwait(false);

            var producer = new RabbitMQProducer<T>(
                producingChannel,
                _serializerProvider,
                _loggerFactory.CreateLogger<RabbitMQProducer<T>>());
            
            _logger.LogInformation("Created a new RabbitMQ producer: {RabbitMQProducer}", producer);

            return producer;
        }
    }
}