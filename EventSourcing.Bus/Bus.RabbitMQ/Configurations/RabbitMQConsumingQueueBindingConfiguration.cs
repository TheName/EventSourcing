namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal class RabbitMQConsumingQueueBindingConfiguration : IRabbitMQConsumingQueueBindingConfiguration
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public ushort PrefetchCount { get; set; }
    }
}