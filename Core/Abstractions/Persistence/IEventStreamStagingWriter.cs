using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.ValueObjects;
using EventSourcing.ValueObjects;

namespace EventSourcing.Persistence
{
    /// <summary>
    /// The event stream staging writer used to modify staged events.
    /// </summary>
    public interface IEventStreamStagingWriter
    {
        /// <summary>
        /// Writes <paramref name="eventStreamEntries"/> and returns a single <see cref="EventStreamStagingId"/> under which the descriptors are assigned.
        /// </summary>
        /// <param name="eventStreamEntries">
        /// The <see cref="EventStreamEntries"/> that should be stored in staging.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamStagingId"/> that identifies stored descriptors saved as staging events.
        /// </returns>
        Task<EventStreamStagingId> WriteAsync(
            EventStreamEntries eventStreamEntries,
            CancellationToken cancellationToken);

        /// <summary>
        /// Marks staging events identified by <see cref="EventStreamStagingId"/> as published.
        /// </summary>
        /// <param name="streamStagingId">
        /// The <see cref="EventStreamStagingId"/> that identifies stored descriptors saved as staging events.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The task representing the marking.
        /// </returns>
        Task MarkAsPublishedAsync(EventStreamStagingId streamStagingId, CancellationToken cancellationToken);

        /// <summary>
        /// Marks staging events identified by <see cref="EventStreamStagingId"/> as failed to store.
        /// </summary>
        /// <param name="streamStagingId">
        /// The <see cref="EventStreamStagingId"/> that identifies stored descriptors saved as staging events.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The task representing the marking.
        /// </returns>
        Task MarkAsFailedToStoreAsync(EventStreamStagingId streamStagingId, CancellationToken cancellationToken);
    }
}
