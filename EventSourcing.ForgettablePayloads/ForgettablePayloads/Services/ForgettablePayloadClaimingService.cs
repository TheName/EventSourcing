using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Services
{
    internal class ForgettablePayloadClaimingService : IForgettablePayloadClaimingService
    {
        private readonly IForgettablePayloadStorageWriter _forgettablePayloadStorageWriter;

        public ForgettablePayloadClaimingService(IForgettablePayloadStorageWriter forgettablePayloadStorageWriter)
        {
            _forgettablePayloadStorageWriter = forgettablePayloadStorageWriter ?? throw new ArgumentNullException(nameof(forgettablePayloadStorageWriter));
        }

        public async Task<ForgettablePayloadDescriptor> ClaimAsync(
            ForgettablePayloadDescriptor payloadDescriptor,
            CancellationToken cancellationToken)
        {
            if (payloadDescriptor == null)
            {
                throw new ArgumentNullException(nameof(payloadDescriptor));
            }
            
            var metadata = payloadDescriptor.ToMetadata();
            var contentDescriptor = payloadDescriptor.ToContentDescriptor();
            
            var updatedMetadata = metadata.CreateUpdated(ForgettablePayloadState.CreatedAndClaimed);
            var updatedDescriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                updatedMetadata,
                contentDescriptor);
            
            await _forgettablePayloadStorageWriter.UpdateAsync(updatedDescriptor, cancellationToken)
                .ConfigureAwait(false);

            return updatedDescriptor;
        }
    }
}