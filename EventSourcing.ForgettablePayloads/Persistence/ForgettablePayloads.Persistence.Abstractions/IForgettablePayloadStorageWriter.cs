using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Persistence
{
    /// <summary>
    /// Storage writer for <see cref="ForgettablePayload"/>
    /// </summary>
    public interface IForgettablePayloadStorageWriter
    {
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
        /// The <see cref="Task"/>
        /// </returns>
        Task InsertAsync(
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
        /// The <see cref="Task"/>
        /// </returns>
        Task UpdateAsync(
            ForgettablePayloadDescriptor forgettablePayloadDescriptor,
            CancellationToken cancellationToken);
    }
}
