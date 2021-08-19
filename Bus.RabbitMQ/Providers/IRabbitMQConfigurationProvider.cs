namespace EventSourcing.Bus.RabbitMQ.Providers
{
    internal interface IRabbitMQConfigurationProvider
    {
        string ExchangeName { get; }
        string QueueName { get; }
        string RoutingKey { get; }
    }
}