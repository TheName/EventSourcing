using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Persistence.Abstractions;

namespace EventSourcing.Persistence
{
    /// <summary>
    /// Repository that writes to EventStreamStaging
    /// </summary>
    public interface IEventStreamStagingWriteRepository
    {
        /// <summary>
        /// Inserts event stream entries under given staging id to EventStreamStaging
        /// </summary>
        /// <param name="stagingId">
        /// The <see cref="EventStreamStagingId"/>
        /// </param>
        /// <param name="entries">
        /// The <see cref="EventStreamEntries"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task InsertAsync(
            EventStreamStagingId stagingId,
            EventStreamEntries entries,
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