using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Reconciliation
{
    /// <summary>
    /// The reconciliation job
    /// </summary>
    public interface IReconciliationJob
    {
        /// <summary>
        /// Executes reconciliation job
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the job.
        /// </returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
