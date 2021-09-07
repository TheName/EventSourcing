using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.Abstractions
{
    /// <summary>
    /// Allows to start and stop consuming events from the EventBus.
    /// </summary>
    public interface IEventSourcingBusConsumer
    {
        /// <summary>
        /// Starts consuming thread.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that allows to cancel the action of starting to consume events.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing starting of consuming.
        /// </returns>
        Task StartConsuming(CancellationToken cancellationToken);

        /// <summary>
        /// Stops consuming thread.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that allows to cancel the action of stopping to consume events.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing stopping of consuming.
        /// </returns>
        Task StopConsuming(CancellationToken cancellationToken);
    }
}