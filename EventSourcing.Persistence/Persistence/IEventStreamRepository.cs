using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions.Enums;

namespace EventSourcing.Persistence
{
    /// <summary>
    /// Repository of EventStream
    /// </summary>
    public interface IEventStreamRepository
    {
        /// <summary>
        /// Writes event stream entries to the EventStream
        /// </summary>
        /// <param name="eventStreamEntries">
        /// The <see cref="EventStreamEntries"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamWriteResult"/>
        /// </returns>
        Task<EventStreamWriteResult> WriteAsync(EventStreamEntries eventStreamEntries, CancellationToken cancellationToken);

        /// <summary>
        /// Reads event stream entries from the EventStream
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntries"/>.
        /// </returns>
        Task<EventStreamEntries> ReadAsync(EventStreamId streamId, CancellationToken cancellationToken);
    }
}