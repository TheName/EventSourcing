using EventSourcing.ValueObjects;

namespace EventSourcing.Serialization
{
    internal class EventSourcingSerializationConfiguration : IEventSourcingSerializationConfiguration
    {
        public SerializationFormat EventContentSerializationFormat { get; set; }

        public SerializationFormat BusSerializationFormat { get; set; }

        public SerializationFormat ForgettablePayloadSerializationFormat { get; set; }
    }
}
