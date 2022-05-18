using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Abstractions.Services
{
    /// <summary>
    /// Loads <see cref="ForgettablePayloadDescriptor"/> object
    /// </summary>
    public interface IForgettablePayloadDescriptorLoader
    {
        /// <summary>
        /// Loads <see cref="ForgettablePayloadDescriptor"/> object
        /// </summary>
        /// <param name="forgettablePayloadId">
        /// The <see cref="ForgettablePayloadId"/> to load
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with result of <see cref="ForgettablePayloadDescriptor"/> representing forgettable payload identified by <paramref name="forgettablePayloadId"/>
        /// </returns>
        Task<ForgettablePayloadDescriptor> LoadAsync(ForgettablePayloadId forgettablePayloadId, CancellationToken cancellationToken);
    }
}