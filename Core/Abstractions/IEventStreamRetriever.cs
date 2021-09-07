using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Retrieves an <see cref="EventStream"/>.
    /// </summary>
    public interface IEventStreamRetriever
    {
        /// <summary>
        /// Retrieves an <see cref="EventStream"/> of provided <paramref name="streamId"/>.
        /// </summary>
        /// <param name="streamId">
        /// An instance of <see cref="EventStreamId"/> that identifies the stream to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EventStream"/> with retrieved events.
        /// </returns>
        Task<EventStream> RetrieveAsync(EventStreamId streamId, CancellationToken cancellationToken);
    }
}