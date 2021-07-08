using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;

namespace EventSourcing.Persistence.Abstractions
{
    /// <summary>
    /// The event stream writer used to modify event source.
    /// </summary>
    public interface IEventStreamWriter
    {
        /// <summary>
        /// Writes <paramref name="eventDescriptors"/> and returns a single <see cref="EventStreamWriteResult"/> that represents the writing result.
        /// </summary>
        /// <param name="eventDescriptors">
        /// The <see cref="IEnumerable{T}"/> of <see cref="EventStreamEventDescriptor"/> that should be stored.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamWriteResult"/> that represents the writing result.
        /// </returns>
        Task<EventStreamWriteResult> WriteAsync(
            IReadOnlyList<EventStreamEventDescriptor> eventDescriptors,
            CancellationToken cancellationToken);
    }
}