using System;
using EventSourcing.Bus.RabbitMQ.Configurations;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMQConnectionFactory> _logger;

        public RabbitMQConnectionFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<RabbitMQConnectionFactory>();
        }
        
        public IRabbitMQConnection Create(IRabbitMQConnectionConfiguration connectionConfiguration)
        {
            var libraryConnectionFactory = new ConnectionFactory
            {
                ClientProvidedName = connectionConfiguration.ClientName,
                DispatchConsumersAsync = true,
                ConsumerDispatchConcurrency = connectionConfiguration.ConsumerDispatchConcurrency,
                RequestedHeartbeat = connectionConfiguration.RequestedHeartbeat,
                Uri = connectionConfiguration.Uri,
                UseBackgroundThreadsForIO = true
            };

            IConnection libraryConnection;
            try
            {
                libraryConnection = libraryConnectionFactory.CreateConnection();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Failed to create RabbitMQ connection. Configuration: {@RabbitMQConnectionConfiguration}",
                    connectionConfiguration);
                
                throw;
            }

            var loggingConnection = new RabbitMQConnection(
                libraryConnection,
                _loggerFactory);
            
            loggingConnection.AttachEventLoggingHandlers();
            
            _logger.LogInformation("Created a new RabbitMQ connection: {RabbitMQConnection}", loggingConnection);

            return loggingConnection;
        }
    }
}