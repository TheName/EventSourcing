using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ValueObjects;

namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// Represents forgettable payload's metadata.
    /// </summary>
    public class ForgettablePayloadMetadata
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
        /// The <see cref="ForgettablePayloadId"/>
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
        /// Creates a new instance of <see cref="ForgettablePayloadMetadata"/>
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>
        /// </param>
        /// <param name="eventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>
        /// </param>
        /// <param name="payloadId">
        /// The <see cref="ForgettablePayloadId"/>
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
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the provided values is null
        /// </exception>
        public ForgettablePayloadMetadata(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence)
        {
            EventStreamId = eventStreamId ?? throw new ArgumentNullException(nameof(eventStreamId));
            EventStreamEntryId = eventStreamEntryId ?? throw new ArgumentNullException(nameof(eventStreamEntryId));
            PayloadId = payloadId ?? throw new ArgumentNullException(nameof(payloadId));
            PayloadState = payloadState ?? throw new ArgumentNullException(nameof(payloadState));
            PayloadCreationTime = payloadCreationTime ?? throw new ArgumentNullException(nameof(payloadCreationTime));
            PayloadLastModifiedTime = payloadLastModifiedTime ?? throw new ArgumentNullException(nameof(payloadLastModifiedTime));
            PayloadSequence = payloadSequence ?? throw new ArgumentNullException(nameof(payloadSequence));
        }

        /// <summary>
        /// Create updated metadata with provided state
        /// </summary>
        /// <param name="newPayloadState">
        /// The <see cref="ForgettablePayloadState"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadMetadata"/>
        /// </returns>
        public ForgettablePayloadMetadata CreateUpdated(ForgettablePayloadState newPayloadState)
        {
            return new ForgettablePayloadMetadata(
                EventStreamId,
                EventStreamEntryId,
                PayloadId,
                newPayloadState,
                PayloadCreationTime,
                ForgettablePayloadLastModifiedTime.Now(),
                PayloadSequence + 1);
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="ForgettablePayloadMetadata"/>.
        /// </param>
        /// <param name="otherMetadata">
        /// The <see cref="ForgettablePayloadMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="metadata"/> and <paramref name="otherMetadata"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadMetadata metadata, ForgettablePayloadMetadata otherMetadata) =>
            Equals(metadata, otherMetadata);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="ForgettablePayloadMetadata"/>.
        /// </param>
        /// <param name="otherMetadata">
        /// The <see cref="ForgettablePayloadMetadata"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="metadata"/> and <paramref name="otherMetadata"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadMetadata metadata, ForgettablePayloadMetadata otherMetadata) =>
            !(metadata == otherMetadata);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadMetadata other &&
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
            $"Forgettable payload's metadata related to event stream id {EventStreamId} and entry id {EventStreamEntryId}, Payload Id {PayloadId}, Payload State: {PayloadState}, Payload Creation Time: {PayloadCreationTime}, Payload Last Modified Time: {PayloadLastModifiedTime}, Payload Sequence: {PayloadSequence}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return EventStreamId;
            yield return EventStreamEntryId;
            yield return PayloadId;
            yield return PayloadState;
            yield return PayloadCreationTime;
            yield return PayloadLastModifiedTime;
            yield return PayloadSequence;
        }
    }
}
