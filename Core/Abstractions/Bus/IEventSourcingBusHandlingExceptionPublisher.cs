using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Exceptions;

namespace EventSourcing.Bus
{
    /// <summary>
    /// Publishes event stream entry handling exception to the event sourcing bus error queue.
    /// </summary>
    public interface IEventSourcingBusHandlingExceptionPublisher
    {
        /// <summary>
        /// Publishes <paramref name="entryHandlingException"/> to the event sourcing bus error queue.
        /// </summary>
        /// <param name="entryHandlingException">
        /// The <see cref="EventStreamEntryHandlingException"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task PublishAsync(
            EventStreamEntryHandlingException entryHandlingException,
            CancellationToken cancellationToken);
    }
}
