using RabbitMQ.Client;

namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal interface IRabbitMQConnectionProvider 
    {
        IConnection Connection { get; }

        void Disconnect();
    }
}