using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.ForgettablePayloads.Cleanup
{
    /// <summary>
    /// A cleanup job that forgets payloads that were not claimed for too long
    /// </summary>
    public interface IUnclaimedForgettablePayloadsCleanupJob
    {
        /// <summary>
        /// Executes cleanup job that forgets payloads that were not claimed for too long
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the job.
        /// </returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
