namespace EventSourcing.Bus.RabbitMQ.Abstractions.Configurations
{
    internal interface IRabbitMQConsumingChannelConfiguration
    {
        string ExchangeName { get; }
        
        string QueueName { get; }
        
        string RoutingKey { get; }
        
        string ExchangeType { get; }
        
        ushort PrefetchCount { get; }
    }
}