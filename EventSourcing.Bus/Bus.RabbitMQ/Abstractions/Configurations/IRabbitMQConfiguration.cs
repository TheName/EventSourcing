namespace EventSourcing.Bus.RabbitMQ.Abstractions.Configurations
{
    internal interface IRabbitMQConfiguration
    {
        string ConnectionString { get; }
    }
}