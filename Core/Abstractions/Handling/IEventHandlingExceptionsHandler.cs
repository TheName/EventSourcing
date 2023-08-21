using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Exceptions;
using EventSourcing.ValueObjects;

namespace EventSourcing.Handling
{
    /// <summary>
    /// Handles exceptions thrown during handling of an <see cref="EventStreamEntry"/>.
    /// </summary>
    public interface IEventHandlingExceptionsHandler
    {
        /// <summary>
        /// Handles exception thrown during handling of <see cref="EventStreamEntry"/>.
        /// </summary>
        /// <param name="entryHandlingException">
        /// The <see cref="EventStreamEntryHandlingException"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the action.
        /// </returns>
        Task HandleAsync(EventStreamEntryHandlingException entryHandlingException, CancellationToken cancellationToken);
    }
}
