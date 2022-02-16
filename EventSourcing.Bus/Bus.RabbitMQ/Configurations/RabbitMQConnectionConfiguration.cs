using System;

namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal class RabbitMQConnectionConfiguration : IRabbitMQConnectionConfiguration
    {
        public string ClientName { get; }
        public TimeSpan RequestedHeartbeat { get; }
        public Uri Uri { get; }
        public int ConsumerDispatchConcurrency { get; }

        public RabbitMQConnectionConfiguration(
            string clientName,
            TimeSpan requestedHeartbeat,
            Uri uri,
            int consumerDispatchConcurrency)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException("Client name cannot be null or whitespace", nameof(clientName));
            }
            
            ClientName = clientName;
            RequestedHeartbeat = requestedHeartbeat;
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));

            if (consumerDispatchConcurrency < 1)
            {
                throw new ArgumentException("Consumer dispatch concurrency cannot be lower than 1",
                    nameof(consumerDispatchConcurrency));
            }
            
            ConsumerDispatchConcurrency = consumerDispatchConcurrency;
        }
    }
}