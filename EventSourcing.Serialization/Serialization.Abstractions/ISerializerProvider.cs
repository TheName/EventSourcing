using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Abstractions
{
    /// <summary>
    /// Provides instances of <see cref="ISerializer"/>.
    /// </summary>
    public interface ISerializerProvider
    {
        /// <summary>
        /// Gets <see cref="ISerializer"/> configured to be used for serialization of <see cref="EventStreamEventContent"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ISerializer"/>.
        /// </returns>
        ISerializer GetEventContentSerializer();
        
        /// <summary>
        /// Gets <see cref="ISerializer"/> configured to be used for serialization by bus package.
        /// </summary>
        /// <returns>
        /// The <see cref="ISerializer"/>.
        /// </returns>
        ISerializer GetBusSerializer();
        
        /// <summary>
        /// Gets <see cref="ISerializer"/> registered for provided <paramref name="serializationFormat"/>.
        /// </summary>
        /// <param name="serializationFormat">
        /// The <see cref="SerializationFormat"/> that returned <see cref="ISerializer"/> should be handling.
        /// </param>
        /// <returns>
        /// The <see cref="ISerializer"/>.
        /// </returns>
        ISerializer GetSerializer(SerializationFormat serializationFormat);
    }
}