﻿using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;

namespace EventSourcing.Bus.RabbitMQ.Configurations
{
    internal class RabbitMQConfiguration : IRabbitMQConfiguration
    {
        public string ConnectionString { get; set; }
    }
}