using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ValueObjects;

namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// Represents a serialized forgettable payload with type and serialization information.
    /// </summary>
    public class ForgettablePayloadDescriptor
    {
        /// <summary>
        /// The <see cref="EventStreamId"/>
        /// </summary>
        public EventStreamId EventStreamId { get; }

        /// <summary>
        /// The <see cref="EventStreamEntryId"/>
        /// </summary>
        public EventStreamEntryId EventStreamEntryId { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadId"/>.
        /// </summary>
        public ForgettablePayloadId PayloadId { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadState"/>
        /// </summary>
        public ForgettablePayloadState PayloadState { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadCreationTime"/>
        /// </summary>
        public ForgettablePayloadCreationTime PayloadCreationTime { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>
        /// </summary>
        public ForgettablePayloadLastModifiedTime PayloadLastModifiedTime { get; }

        /// <summary>
        /// The <see cref="ForgettablePayloadSequence"/>
        /// </summary>
        public ForgettablePayloadSequence PayloadSequence { get; }

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
        /// Initializes a new instance of the <see cref="ForgettablePayloadContent"/> class.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="ForgettablePayloadMetadata"/>
        /// </param>
        /// <param name="contentDescriptor">
        /// The <see cref="ForgettablePayloadContentDescriptor"/>
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="ForgettablePayloadContent"/> class.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public static ForgettablePayloadDescriptor CreateFromMetadataAndContentDescriptor(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadContentDescriptor contentDescriptor)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (contentDescriptor == null)
            {
                throw new ArgumentNullException(nameof(contentDescriptor));
            }

            return new ForgettablePayloadDescriptor(
                metadata.EventStreamId,
                metadata.EventStreamEntryId,
                metadata.PayloadId,
                metadata.PayloadState,
                metadata.PayloadCreationTime,
                metadata.PayloadLastModifiedTime,
                metadata.PayloadSequence,
                contentDescriptor.PayloadContent,
                contentDescriptor.PayloadContentSerializationFormat,
                contentDescriptor.PayloadTypeIdentifier,
                contentDescriptor.PayloadTypeIdentifierFormat);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettablePayloadContent"/> class.
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="eventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>
        /// </param>
        /// <param name="payloadId">
        /// The <see cref="ForgettablePayloadId"/>.
        /// </param>
        /// <param name="payloadState">
        /// The <see cref="ForgettablePayloadState"/>
        /// </param>
        /// <param name="payloadCreationTime">
        /// The <see cref="ForgettablePayloadCreationTime"/>
        /// </param>
        /// <param name="payloadLastModifiedTime">
        /// The <see cref="ForgettablePayloadLastModifiedTime"/>
        /// </param>
        /// <param name="payloadSequence">
        /// The <see cref="ForgettablePayloadSequence"/>
        /// </param>
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
        public ForgettablePayloadDescriptor(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            EventStreamId = eventStreamId ?? throw new ArgumentNullException(nameof(eventStreamId));
            EventStreamEntryId = eventStreamEntryId ?? throw new ArgumentNullException(nameof(eventStreamEntryId));
            PayloadId = payloadId ?? throw new ArgumentNullException(nameof(payloadId));
            PayloadState = payloadState ?? throw new ArgumentNullException(nameof(payloadState));
            PayloadCreationTime = payloadCreationTime ?? throw new ArgumentNullException(nameof(payloadCreationTime));
            PayloadLastModifiedTime = payloadLastModifiedTime ?? throw new ArgumentNullException(nameof(payloadLastModifiedTime));
            PayloadSequence = payloadSequence ?? throw new ArgumentNullException(nameof(payloadSequence));
            PayloadContent = payloadContent ?? throw new ArgumentNullException(nameof(payloadContent));
            PayloadContentSerializationFormat = payloadContentSerializationFormat ?? throw new ArgumentNullException(nameof(payloadContentSerializationFormat));
            PayloadTypeIdentifier = payloadTypeIdentifier ?? throw new ArgumentNullException(nameof(payloadTypeIdentifier));
            PayloadTypeIdentifierFormat = payloadTypeIdentifierFormat ?? throw new ArgumentNullException(nameof(payloadTypeIdentifierFormat));
        }

        /// <summary>
        /// Creates a <see cref="ForgettablePayloadMetadata"/> instance using values from this descriptor
        /// </summary>
        /// <returns>
        /// The <see cref="ForgettablePayloadMetadata"/>
        /// </returns>
        public ForgettablePayloadMetadata ToMetadata()
        {
            return new ForgettablePayloadMetadata(
                EventStreamId,
                EventStreamEntryId,
                PayloadId,
                PayloadState,
                PayloadCreationTime,
                PayloadLastModifiedTime,
                PayloadSequence);
        }

        /// <summary>
        /// Creates a <see cref="ForgettablePayloadContentDescriptor"/> instance using values from this descriptor
        /// </summary>
        /// <returns>
        /// The <see cref="ForgettablePayloadContentDescriptor"/>
        /// </returns>
        public ForgettablePayloadContentDescriptor ToContentDescriptor()
        {
            return new ForgettablePayloadContentDescriptor(
                PayloadContent,
                PayloadContentSerializationFormat,
                PayloadTypeIdentifier,
                PayloadTypeIdentifierFormat);
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgettablePayloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/>.
        /// </param>
        /// <param name="otherForgettablePayloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadDescriptor"/> and <paramref name="otherForgettablePayloadDescriptor"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadDescriptor forgettablePayloadDescriptor, ForgettablePayloadDescriptor otherForgettablePayloadDescriptor) =>
            Equals(forgettablePayloadDescriptor, otherForgettablePayloadDescriptor);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgettablePayloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/>.
        /// </param>
        /// <param name="otherForgettablePayloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadDescriptor"/> and <paramref name="otherForgettablePayloadDescriptor"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadDescriptor forgettablePayloadDescriptor, ForgettablePayloadDescriptor otherForgettablePayloadDescriptor) =>
            !(forgettablePayloadDescriptor == otherForgettablePayloadDescriptor);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadDescriptor other &&
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
            $"Forgettable payload attached to event stream id {EventStreamId} and entry id {EventStreamEntryId}, Payload Id {PayloadId}, Payload State: {PayloadState}, Payload Creation Time: {PayloadCreationTime}, Payload Last Modified Time: {PayloadLastModifiedTime}, Payload Sequence: {PayloadSequence}, Payload Content: {PayloadContent}, Payload Content Serialization Format: {PayloadContentSerializationFormat}, Payload Type Identifier: {PayloadTypeIdentifier}, Payload Type Identifier Format: {PayloadTypeIdentifierFormat}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return EventStreamId;
            yield return EventStreamEntryId;
            yield return PayloadId;
            yield return PayloadState;
            yield return PayloadCreationTime;
            yield return PayloadLastModifiedTime;
            yield return PayloadSequence;
            yield return PayloadContent;
            yield return PayloadContentSerializationFormat;
            yield return PayloadTypeIdentifier;
            yield return PayloadTypeIdentifierFormat;
        }
    }
}
