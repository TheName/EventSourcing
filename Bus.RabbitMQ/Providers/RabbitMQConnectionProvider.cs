using System;
using EventSourcing.Bus.RabbitMQ.Factories;
using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal class RabbitMQConnectionProvider : IRabbitMQConnectionProvider
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
        
        public void Disconnect()
        {
            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Close();
            }
        }
    }
}