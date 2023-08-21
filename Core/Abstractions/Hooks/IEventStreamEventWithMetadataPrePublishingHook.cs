using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ValueObjects;

namespace EventSourcing.Hooks
{
    /// <summary>
    /// A hook invoked before publishing event with metadata
    /// </summary>
    public interface IEventStreamEventWithMetadataPrePublishingHook
    {
        /// <summary>
        /// Called with <see cref="EventStreamEventWithMetadata"/> before trying to publish it
        /// </summary>
        /// <param name="eventWithMetadata">
        /// The <see cref="EventStreamEventWithMetadata"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task PreEventStreamEventWithMetadataPublishHookAsync(EventStreamEventWithMetadata eventWithMetadata, CancellationToken cancellationToken);
    }
}
