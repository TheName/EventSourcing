using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Services
{
    /// <summary>
    /// Claiming service
    /// </summary>
    public interface IForgettablePayloadClaimingService
    {
        /// <summary>
        /// Claims <paramref name="payloadDescriptor"/>
        /// </summary>
        /// <param name="payloadDescriptor">
        /// The <see cref="ForgettablePayloadDescriptor"/> that should be claimed
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with result of updated <see cref="ForgettablePayloadDescriptor"/>
        /// </returns>
        Task<ForgettablePayloadDescriptor> ClaimAsync(ForgettablePayloadDescriptor payloadDescriptor, CancellationToken cancellationToken);
    }
}
