using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream publisher used to store and publish (to event bus) appended entries from <see cref="AppendableEventStream"/>. 
    /// </summary>
    public interface IEventStreamPublisher
    {
        /// <summary>
        /// Stores appended entries in the event source and publishes them to event bus.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="PublishableEventStream"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// A task that represents the publishing.
        /// </returns>
        /// <exception cref="EventStreamAppendingFailedException">
        /// Thrown when an unknown failure to store entries in the event source happens.
        /// </exception>
        /// <exception cref="EventStreamOptimisticConcurrencyException">
        /// Thrown when storing the entries in the event source is not possible due to the fact that other entries with same sequences are already stored.
        /// </exception>
        Task PublishAsync(PublishableEventStream stream, CancellationToken cancellationToken);
    }
}