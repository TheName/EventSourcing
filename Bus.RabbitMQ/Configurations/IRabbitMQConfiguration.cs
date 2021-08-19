namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal interface IRabbitMQConfiguration
    {
        string ConnectionString { get; }
    }
}