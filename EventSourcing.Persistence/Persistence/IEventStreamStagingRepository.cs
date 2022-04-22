using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Persistence
{
    /// <summary>
    /// Repository that writes to and reads from EventStreamStaging
    /// </summary>
    public interface IEventStreamStagingRepository
    {
        /// <summary>
        /// Reads event stream staged entries from EventStreamStaging
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="IReadOnlyCollection{T}"/> of <see cref="EventStreamStagedEntries"/>
        /// </returns>
        Task<IReadOnlyCollection<EventStreamStagedEntries>> SelectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Reads event stream staged entries from EventStreamStaging with provided staging id or null
        /// </summary>
        /// <param name="stagingId">
        /// The <see cref="EventStreamStagingId"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamStagedEntries"/>
        /// </returns>
        Task<EventStreamStagedEntries> SelectAsync(EventStreamStagingId stagingId, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts event stream entries under given staging id to EventStreamStaging
        /// </summary>
        /// <param name="stagedEntries">
        /// The <see cref="EventStreamStagedEntries"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task InsertAsync(
            EventStreamStagedEntries stagedEntries,
            CancellationToken cancellationToken);

        /// <summary>
        /// Deletes entries under given staging id
        /// </summary>
        /// <param name="stagingId">
        /// The <see cref="EventStreamStagingId"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task DeleteAsync(EventStreamStagingId stagingId, CancellationToken cancellationToken);
    }
}