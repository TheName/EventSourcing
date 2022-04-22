using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Abstractions
{
    /// <summary>
    /// Configuration for serialization of EventSourcing
    /// </summary>
    public interface IEventSourcingSerializationConfiguration
    {
        /// <summary>
        /// The <see cref="SerializationFormat"/> that should be used to serialize <see cref="EventStreamEventContent"/>.
        /// </summary>
        SerializationFormat EventContentSerializationFormat { get; }
        
        /// <summary>
        /// The <see cref="SerializationFormat"/> that should be used for serialization by bus package
        /// </summary>
        SerializationFormat BusSerializationFormat { get; }
    }
}