using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The converter used to convert between <see cref="EventStreamEvent"/> and <see cref="EventStreamEventDescriptor"/>.
    /// </summary>
    public interface IEventStreamEventConverter
    {
        /// <summary>
        /// Converts <paramref name="event"/> to <see cref="EventStreamEventDescriptor"/>.
        /// </summary>
        /// <param name="event">
        /// The <see cref="EventStreamEvent"/> that should be converted.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <paramref name="event"/> converted to <see cref="EventStreamEventDescriptor"/>. 
        /// </returns>
        Task<EventStreamEventDescriptor> ToDescriptorAsync(
            EventStreamEvent @event,
            CancellationToken cancellationToken);

        /// <summary>
        /// Converts <paramref name="descriptor"/> to <see cref="EventStreamEvent"/>.
        /// </summary>
        /// <param name="descriptor">
        /// The <see cref="EventStreamEventDescriptor"/> that should be converted.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <paramref name="descriptor"/> converted to <see cref="EventStreamEvent"/>.
        /// </returns>
        Task<EventStreamEvent> FromDescriptorAsync(
            EventStreamEventDescriptor descriptor,
            CancellationToken cancellationToken);
    }
}