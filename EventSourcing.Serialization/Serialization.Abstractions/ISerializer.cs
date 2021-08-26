namespace EventSourcing.Serialization.Abstractions
{
    /// <summary>
    /// The serializer
    /// </summary>
    public interface ISerializer
    {
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
    }
}