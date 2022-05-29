using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Persistence
{
    /// <summary>
    /// Repository of ForgettablePayloadStorage
    /// </summary>
    public interface IForgettablePayloadStorageRepository
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
        /// <param name="forgettablePayloadState">
        /// The <see cref="ForgettablePayloadState"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadDescriptor"/> collection
        /// </returns>
        Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(
            ForgettablePayloadState forgettablePayloadState,
            CancellationToken cancellationToken);
        
        /// <summary>
        /// Inserts forgettable payload to storage
        /// </summary>
        /// <param name="forgettablePayloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with <see cref="bool"/> result determining whether insertion was successful
        /// </returns>
        Task<bool> TryInsertAsync(
            ForgettablePayloadDescriptor forgettablePayloadDescriptor,
            CancellationToken cancellationToken);

        /// <summary>
        /// Updates forgettable payload to storage if currently stored version has a sequence smaller by one from the one provided in <paramref name="forgettablePayloadDescriptor"/>
        /// </summary>
        /// <param name="forgettablePayloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with <see cref="bool"/> result determining whether update was successful
        /// </returns>
        Task<bool> TryUpdateAsync(
            ForgettablePayloadDescriptor forgettablePayloadDescriptor,
            CancellationToken cancellationToken);
    }
}