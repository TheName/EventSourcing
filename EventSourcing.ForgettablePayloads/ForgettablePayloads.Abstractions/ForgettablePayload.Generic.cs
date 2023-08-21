using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads
{
    /// <summary>
    /// The forgettable payload
    /// </summary>
    public class ForgettablePayload<T> : ForgettablePayload
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayload"/>
        /// </summary>
        /// <param name="payloadId">
        /// The <see cref="ForgettablePayloadId"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <see cref="ForgettablePayloadId"/> is null.
        /// </exception>
        public ForgettablePayload(ForgettablePayloadId payloadId) : base(payloadId)
        {
        }

        /// <summary>
        /// Gets the actual payload
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The actual payload
        /// </returns>
        public new async Task<T> GetPayloadAsync(CancellationToken cancellationToken)
        {
            var payloadObject = await base.GetPayloadAsync(cancellationToken).ConfigureAwait(false);

            return (T)payloadObject;
        }
    }
}
