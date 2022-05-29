using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Persistence.Abstractions
{
    /// <summary>
    /// Storage reader for <see cref="ForgettablePayload"/>
    /// </summary>
    public interface IForgettablePayloadStorageReader
    {
        /// <summary>
        /// Reads <see cref="ForgettablePayloadDescriptor"/>s from storage that are assigned to provided <see cref="EventStreamId"/>
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadDescriptor"/> collection
        /// </returns>
        Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(EventStreamId eventStreamId, CancellationToken cancellationToken);

        /// <summary>
        /// Reads <see cref="ForgettablePayloadDescriptor"/>s from storage that are assigned to provided <see cref="EventStreamId"/> and <see cref="EventStreamEntryId"/>
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>
        /// </param>
        /// <param name="eventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadDescriptor"/> collection
        /// </returns>
        Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            CancellationToken cancellationToken);
        
        /// <summary>
        /// Reads <see cref="ForgettablePayloadDescriptor"/> from storage
        /// </summary>
        /// <param name="forgettablePayloadId">
        /// The <see cref="ForgettablePayloadId"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadDescriptor"/>
        /// </returns>
        Task<ForgettablePayloadDescriptor> ReadAsync(
            ForgettablePayloadId forgettablePayloadId,
            CancellationToken cancellationToken);
        
        /// <summary>
        /// Reads unclaimed <see cref="ForgettablePayloadDescriptor"/>s from storage
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadDescriptor"/> collection
        /// </returns>
        Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadUnclaimedAsync(CancellationToken cancellationToken);
    }
}