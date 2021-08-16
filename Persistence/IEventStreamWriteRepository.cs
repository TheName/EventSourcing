using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Persistence.Abstractions;

namespace EventSourcing.Persistence
{
    /// <summary>
    /// Repository that writes to EventStream
    /// </summary>
    public interface IEventStreamWriteRepository
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
        Task<EventStreamWriteResult> WriteAsync(
            EventStreamEntries eventStreamEntries,
            CancellationToken cancellationToken);
    }
}