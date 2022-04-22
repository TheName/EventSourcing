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

        /// <summary>
        /// Reads events from provided <paramref name="streamId"/> with sequences in the provided range (inclusive)
        /// </summary>
        /// <param name="streamId">
        /// An instance of <see cref="EventStreamId"/> identifying the stream to read events from.
        /// </param>
        /// <param name="minimumSequenceInclusive">
        /// The minimum (inclusive) event's sequence that should be included in the result. 
        /// </param>
        /// <param name="maximumSequenceInclusive">
        /// The maximum (inclusive) event's sequence that should be included in the result.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EventStreamEntries"/> containing entries already stored in persistence layer.
        /// </returns>
        Task<EventStreamEntries> ReadAsync(
            EventStreamId streamId,
            EventStreamEntrySequence minimumSequenceInclusive,
            EventStreamEntrySequence maximumSequenceInclusive,
            CancellationToken cancellationToken);
    }
}