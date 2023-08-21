using System;
using EventSourcing.ValueObjects;

namespace EventSourcing.Serialization
{
    /// <summary>
    /// The serializer
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// The <see cref="ValueObjects.SerializationFormat"/> used by this instance of serializer.
        /// </summary>
        SerializationFormat SerializationFormat { get; }

        /// <summary>
        /// Serializes provided object to a string representation
        /// </summary>
        /// <param name="object">
        /// The object to serialize
        /// </param>
        /// <returns>
        /// A serialized representation of <paramref name="object"/> in the form of string.
        /// </returns>
        string Serialize(object @object);

        /// <summary>
        /// Serializes provided object
        /// </summary>
        /// <param name="object">
        /// The object to serialize
        /// </param>
        /// <returns>
        /// A serialized representation of <paramref name="object"/> in the form of UTF-8 encoded string.
        /// </returns>
        byte[] SerializeToUtf8Bytes(object @object);

        /// <summary>
        /// Deserializes provided <paramref name="serializedObject"/> to object of type <paramref name="objectType"/>
        /// </summary>
        /// <param name="serializedObject">
        /// The serialized object.
        /// </param>
        /// <param name="objectType">
        /// The <see cref="Type"/> that provided <paramref name="serializedObject"/> should be deserialized to.
        /// </param>
        /// <returns>
        /// Provided <paramref name="serializedObject"/> deserialized to type <paramref name="objectType"/>.
        /// </returns>
        object Deserialize(string serializedObject, Type objectType);

        /// <summary>
        /// Deserializes provided <paramref name="serializedObject"/> to object of type <paramref name="objectType"/>
        /// </summary>
        /// <param name="serializedObject">
        /// The serialized object.
        /// </param>
        /// <param name="objectType">
        /// The <see cref="Type"/> that provided <paramref name="serializedObject"/> should be deserialized to.
        /// </param>
        /// <returns>
        /// Provided <paramref name="serializedObject"/> deserialized to type <paramref name="objectType"/>.
        /// </returns>
        object DeserializeFromUtf8Bytes(byte[] serializedObject, Type objectType);
    }
}
