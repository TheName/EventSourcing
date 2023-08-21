using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Aggregates.Publishers
{
    /// <summary>
    /// Publishes event stream aggregates.
    /// </summary>
    public interface IEventStreamAggregatePublisher
    {
        /// <summary>
        /// Publishes provided aggregate.
        /// </summary>
        /// <param name="aggregate">
        /// An event stream aggregate.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the action.
        /// </returns>
        Task PublishAsync(object aggregate, CancellationToken cancellationToken);
    }
}
