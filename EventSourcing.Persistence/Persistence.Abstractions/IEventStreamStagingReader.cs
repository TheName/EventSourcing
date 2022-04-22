using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Persistence.Abstractions
{
    /// <summary>
    /// The event stream staging reader used to read staged events.
    /// </summary>
    public interface IEventStreamStagingReader
    {
        /// <summary>
        /// Reads staged entries that were not marked as published or as deleted.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="IReadOnlyCollection{T}"/> of <see cref="EventStreamStagedEntries"/>
        /// </returns>
        Task<IReadOnlyCollection<EventStreamStagedEntries>> ReadUnmarkedStagedEntriesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Reads staged entries with provided <paramref name="stagingId"/> that were not marked as published or as deleted or null. 
        /// </summary>
        /// <param name="stagingId">
        /// The <see cref="EventStreamStagingId"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="IReadOnlyCollection{T}"/> of <see cref="EventStreamStagedEntries"/>
        /// </returns>
        Task<EventStreamStagedEntries> ReadUnmarkedStagedEntriesAsync(
            EventStreamStagingId stagingId,
            CancellationToken cancellationToken);
    }
}