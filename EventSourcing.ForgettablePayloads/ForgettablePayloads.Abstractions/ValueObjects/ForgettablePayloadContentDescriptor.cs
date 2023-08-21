using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ValueObjects;

namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// Represents a serialized forgettable payload content with type and serialization information.
    /// </summary>
    public class ForgettablePayloadContentDescriptor
    {
        /// <summary>
        /// The serialized <see cref="ForgettablePayloadContent"/> of the payload.
        /// </summary>
        public ForgettablePayloadContent PayloadContent { get; }

        /// <summary>
        /// The <see cref="SerializationFormat"/> used to serialize the <see cref="ForgettablePayloadContent"/>.
        /// </summary>
        public SerializationFormat PayloadContentSerializationFormat { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadTypeIdentifier"/> of the payload.
        /// </summary>
        public ForgettablePayloadTypeIdentifier PayloadTypeIdentifier { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/> of the <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </summary>
        public ForgettablePayloadTypeIdentifierFormat PayloadTypeIdentifierFormat { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettablePayloadContentDescriptor"/> class.
        /// </summary>
        /// <param name="payloadContent">
        /// The serialized <see cref="ForgettablePayloadContent"/> of the payload.
        /// </param>
        /// <param name="payloadContentSerializationFormat">
        /// The <see cref="SerializationFormat"/> used to serialize the <see cref="ForgettablePayloadContent"/>.
        /// </param>
        /// <param name="payloadTypeIdentifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/> of the payload.
        /// </param>
        /// <param name="payloadTypeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/> of the <paramref name="payloadTypeIdentifier"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public ForgettablePayloadContentDescriptor(
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            PayloadContent = payloadContent ?? throw new ArgumentNullException(nameof(payloadContent));
            PayloadContentSerializationFormat = payloadContentSerializationFormat ?? throw new ArgumentNullException(nameof(payloadContentSerializationFormat));
            PayloadTypeIdentifier = payloadTypeIdentifier ?? throw new ArgumentNullException(nameof(payloadTypeIdentifier));
            PayloadTypeIdentifierFormat = payloadTypeIdentifierFormat ?? throw new ArgumentNullException(nameof(payloadTypeIdentifierFormat));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgettablePayloadContentDescriptor">
        /// The <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </param>
        /// <param name="otherForgettablePayloadContentDescriptor">
        /// The <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadContentDescriptor"/> and <paramref name="otherForgettablePayloadContentDescriptor"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadContentDescriptor forgettablePayloadContentDescriptor, ForgettablePayloadContentDescriptor otherForgettablePayloadContentDescriptor) =>
            Equals(forgettablePayloadContentDescriptor, otherForgettablePayloadContentDescriptor);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgettablePayloadContentDescriptor">
        /// The <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </param>
        /// <param name="otherForgettablePayloadContentDescriptor">
        /// The <see cref="ForgettablePayloadContentDescriptor"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadContentDescriptor"/> and <paramref name="otherForgettablePayloadContentDescriptor"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadContentDescriptor forgettablePayloadContentDescriptor, ForgettablePayloadContentDescriptor otherForgettablePayloadContentDescriptor) =>
            !(forgettablePayloadContentDescriptor == otherForgettablePayloadContentDescriptor);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadContentDescriptor other &&
            other.GetPropertiesForHashCode().SequenceEqual(GetPropertiesForHashCode());

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Select(o => o.GetHashCode())
                    .Where(i => i != 0)
                    .Aggregate(17, (current, hashCode) => current * 23 * hashCode);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Forgettable Payload Content: {PayloadContent}, Payload Content Serialization Format: {PayloadContentSerializationFormat}, Payload Type Identifier: {PayloadTypeIdentifier}, Payload Type Identifier Format: {PayloadTypeIdentifierFormat}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return PayloadContent;
            yield return PayloadContentSerializationFormat;
            yield return PayloadTypeIdentifier;
            yield return PayloadTypeIdentifierFormat;
        }
    }
}
