using System;

namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal class RabbitMQProducingQueueBindingConfiguration : IRabbitMQProducingQueueBindingConfiguration
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public TimeSpan PublishingTimeout { get; set; }
    }
}