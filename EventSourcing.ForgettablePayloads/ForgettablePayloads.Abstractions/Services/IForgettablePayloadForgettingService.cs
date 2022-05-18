using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Abstractions.Services
{
    /// <summary>
    /// Forgetting service
    /// </summary>
    public interface IForgettablePayloadForgettingService
    {
        /// <summary>
        /// Forgets <paramref name="payloadDescriptor"/> due to provided <paramref name="forgettingReason"/> and <paramref name="forgettingRequestedBy"/>.
        /// </summary>
        /// <param name="payloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/> that should be forgotten
        /// </param>
        /// <param name="forgettingReason">
        /// The <see cref="ForgettingPayloadReason"/>.
        /// </param>
        /// <param name="forgettingRequestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with result of updated <see cref="ForgettablePayloadDescriptor"/>
        /// </returns>
        Task<ForgettablePayloadDescriptor> ForgetAsync(
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy,
            CancellationToken cancellationToken);
    }
}