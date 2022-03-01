﻿using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Serialization
{
    internal class EventSourcingSerializationConfiguration : IEventSourcingSerializationConfiguration
    {
        public SerializationFormat EventContentSerializationFormat { get; set; }
    }
}