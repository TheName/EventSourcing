using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private readonly Guid _connectionId = Guid.NewGuid();

        public RabbitMQConnection(
            IConnection connection,
            ILoggerFactory loggerFactory)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<RabbitMQConnection>() ?? throw new ArgumentNullException(nameof(_logger));
        }

        public void AttachEventLoggingHandlers()
        {
            _connection.CallbackException += ConnectionOnCallbackException;
            _connection.ConnectionBlocked += ConnectionOnConnectionBlocked;
            _connection.ConnectionUnblocked += ConnectionOnConnectionUnblocked;
            _connection.ConnectionShutdown += ConnectionOnConnectionShutdown;
        }

        public IRabbitMQChannel CreateChannel()
        {
            var model = _connection.CreateModel();
            var channel = new RabbitMQChannel(model, _loggerFactory.CreateLogger<RabbitMQChannel>());
            channel.AttachEventLoggingHandlers();
            
            _logger.LogInformation("Created a new RabbitMQ channel: {RabbitMQChannel} using connection {RabbitMQConnection}", channel, this);

            return channel;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private void ConnectionOnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.LogWarning(
                e.Exception,
                "Connection callback exception. Sender: {ConnectionCallbackExceptionSender}, EventArgs: {CallbackExceptionEventArgs}, Detail: {@CallbackExceptionDetail}, Connection: {RabbitMQConnection}",
                sender,
                e,
                e.Detail,
                this);
        }

        private void ConnectionOnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            _logger.LogDebug(
                "Connection blocked. Sender: {ConnectionBlockedSender}, EventArgs: {@ConnectionBlockedEventArgs}, Connection: {RabbitMQConnection}",
                sender,
                e,
                this);
        }

        private void ConnectionOnConnectionUnblocked(object sender, EventArgs e)
        {
            _logger.LogDebug(
                "Connection unblocked. Sender: {ConnectionUnblockedSender}, EventArgs: {@EventArgs}, Connection: {RabbitMQConnection}",
                sender,
                e,
                this);
        }

        private void ConnectionOnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogDebug(
                "Connection shutdown. Sender: {ConnectionShutdownSender}, EventArgs: {ShutdownEventArgs}, Connection: {RabbitMQConnection}",
                sender,
                e,
                this);
        }

        public override string ToString()
        {
            return $"RabbitMQ Connection with id {_connectionId}. Connection: {_connection}";
        }
    }
}