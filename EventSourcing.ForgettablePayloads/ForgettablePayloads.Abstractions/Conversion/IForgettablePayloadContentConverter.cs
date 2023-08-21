using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Conversion
{
    /// <summary>
    /// Converts objects to <see cref="ForgettablePayloadContentDescriptor"/> and <see cref="ForgettablePayloadContentDescriptor"/> to objects.
    /// </summary>
    public interface IForgettablePayloadContentConverter
    {
        /// <summary>
        /// Converts <paramref name="payload"/> to <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </summary>
        /// <param name="payload">
        /// The actual payload object
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </returns>
        ForgettablePayloadContentDescriptor ToPayloadContentDescriptor(object payload);

        /// <summary>
        /// Converts <see cref="ForgettablePayloadContentDescriptor"/> to actual payload object
        /// </summary>
        /// <param name="payloadContentDescriptor">
        /// The <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </param>
        /// <returns>
        /// The actual payload object
        /// </returns>
        object FromPayloadContentDescriptor(ForgettablePayloadContentDescriptor payloadContentDescriptor);
    }
}
