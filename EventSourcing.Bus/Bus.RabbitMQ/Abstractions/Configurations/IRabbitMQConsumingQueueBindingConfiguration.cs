namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal interface IRabbitMQConsumingQueueBindingConfiguration
    {
        string ExchangeName { get; }
        
        string ExchangeType { get; }
        
        string QueueName { get; }
        
        string RoutingKey { get; }
        
        ushort PrefetchCount { get; }
    }
}