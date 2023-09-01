using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Enums;
using EventSourcing.ValueObjects;

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

        /// <summary>
        /// Reads event stream entries from the EventStream with provided <paramref name="streamId"/>
        /// and sequences in inclusive range of <paramref name="minimumSequenceInclusive"/> and <paramref name="maximumSequenceInclusive"/>.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="minimumSequenceInclusive">
        /// The <see cref="EventStreamEntrySequence"/> representing minimum (inclusive) sequence
        /// </param>
        /// <param name="maximumSequenceInclusive">
        /// The <see cref="EventStreamEntrySequence"/> representing maximum (inclusive) sequence
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntries"/>.
        /// </returns>
        Task<EventStreamEntries> ReadAsync(
            EventStreamId streamId,
            EventStreamEntrySequence minimumSequenceInclusive,
            EventStreamEntrySequence maximumSequenceInclusive,
            CancellationToken cancellationToken);
    }
}
