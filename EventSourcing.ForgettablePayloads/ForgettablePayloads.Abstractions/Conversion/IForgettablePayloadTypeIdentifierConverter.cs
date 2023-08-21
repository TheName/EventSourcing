using System;
using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Conversion
{
    /// <summary>
    /// Converts <see cref="ForgettablePayloadTypeIdentifier"/> to <see cref="Type"/> and <see cref="Type"/> to <see cref="ForgettablePayloadTypeIdentifier"/>.
    /// </summary>
    public interface IForgettablePayloadTypeIdentifierConverter
    {
        /// <summary>
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/> used by this instance of serializer.
        /// </summary>
        ForgettablePayloadTypeIdentifierFormat TypeIdentifierFormat { get; }

        /// <summary>
        /// Converts <paramref name="type"/> to <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> of an event.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadTypeIdentifier"/> that uniquely identifies the <see cref="Type"/> of an event.
        /// </returns>
        ForgettablePayloadTypeIdentifier ToTypeIdentifier(Type type);

        /// <summary>
        /// Converts <paramref name="identifier"/> to <see cref="Type"/>.
        /// </summary>
        /// <param name="identifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/> that uniquely identifies the <see cref="Type"/> of an event.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> of an event.
        /// </returns>
        Type FromTypeIdentifier(ForgettablePayloadTypeIdentifier identifier);
    }
}
