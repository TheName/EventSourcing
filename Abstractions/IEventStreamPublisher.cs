using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Exceptions;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream publisher used to store and publish (to event bus) appended events from <see cref="EventStream"/>. 
    /// </summary>
    public interface IEventStreamPublisher
    {
        /// <summary>
        /// Stores appended events in the event source and publishes them to event bus.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="EventStream"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// A task that represents the publishing.
        /// </returns>
        /// <exception cref="EventStreamAppendingFailedException">
        /// Thrown when an unknown failure to store events in the event source happens.
        /// </exception>
        /// <exception cref="EventStreamOptimisticConcurrencyException">
        /// Thrown when storing the events in the event source is not possible due to the fact that other events with same sequences are already stored.
        /// </exception>
        Task PublishAsync(EventStream stream, CancellationToken cancellationToken);
    }
}