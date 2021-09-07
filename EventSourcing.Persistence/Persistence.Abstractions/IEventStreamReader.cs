using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Persistence.Abstractions
{
    /// <summary>
    /// Reads event stream.
    /// </summary>
    public interface IEventStreamReader
    {
        /// <summary>
        /// Reads events from provided <paramref name="streamId"/>.
        /// </summary>
        /// <param name="streamId">
        /// An instance of <see cref="EventStreamId"/> identifying the stream to read events from.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EventStreamEntries"/> containing entries already stored in persistence layer.
        /// </returns>
        Task<EventStreamEntries> ReadAsync(EventStreamId streamId, CancellationToken cancellationToken);
    }
}