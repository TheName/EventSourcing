using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Abstractions.Services
{
    /// <summary>
    /// A forgetting service for all forgettable payloads assigned to the same event stream
    /// </summary>
    public interface IForgettablePayloadEventStreamForgettingService
    {
        /// <summary>
        /// Forgets all forgettable payloads assigned to the same event stream
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> identifying the event stream that might contain forgettable payloads
        /// </param>
        /// <param name="forgettingPayloadReason">
        /// The <see cref="ForgettingPayloadReason"/> that should be saved as the reason for forgetting each payload.
        /// </param>
        /// <param name="forgettingPayloadRequestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/> that should be saved as the requesting entity for forgetting each payload.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation
        /// </returns>
        Task ForgetAllForgettablePayloadsFromStreamAsync(
            EventStreamId streamId,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            CancellationToken cancellationToken);
    }
}