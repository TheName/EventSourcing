using System;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQConnectionProvider : IRabbitMQConnectionProvider, IDisposable
    {
        private readonly Lazy<IConnection> _lazyConnection;

        public IConnection Connection => _lazyConnection.Value;

        public RabbitMQConnectionProvider(IRabbitMQConnectionFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _lazyConnection = new Lazy<IConnection>(factory.Create);
        }

        public void Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Dispose();
            }
        }
    }
}