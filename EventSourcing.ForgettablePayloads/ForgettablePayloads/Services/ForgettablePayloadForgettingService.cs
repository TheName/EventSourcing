using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Services
{
    internal class ForgettablePayloadForgettingService : IForgettablePayloadForgettingService
    {
        private readonly IForgettablePayloadContentConverter _forgettablePayloadConverter;
        private readonly IForgettablePayloadStorageWriter _forgettablePayloadStorageWriter;

        public ForgettablePayloadForgettingService(
            IForgettablePayloadContentConverter forgettablePayloadConverter,
            IForgettablePayloadStorageWriter forgettablePayloadStorageWriter)
        {
            _forgettablePayloadConverter = forgettablePayloadConverter ?? throw new ArgumentNullException(nameof(forgettablePayloadConverter));
            _forgettablePayloadStorageWriter = forgettablePayloadStorageWriter ?? throw new ArgumentNullException(nameof(forgettablePayloadStorageWriter));
        }
        
        public async Task<ForgettablePayloadDescriptor> ForgetAsync(
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy,
            CancellationToken cancellationToken)
        {
            if (payloadDescriptor == null)
            {
                throw new ArgumentNullException(nameof(payloadDescriptor));
            }

            if (forgettingReason == null)
            {
                throw new ArgumentNullException(nameof(forgettingReason));
            }

            if (forgettingRequestedBy == null)
            {
                throw new ArgumentNullException(nameof(forgettingRequestedBy));
            }
            
            var metadata = payloadDescriptor.ToMetadata();
            var updatedMetadata = metadata.CreateUpdated(ForgettablePayloadState.Forgotten);
            var forgottenPayloadContent = ForgottenPayload.Create(forgettingReason, forgettingRequestedBy);
            var forgottenPayloadContentDescriptor = _forgettablePayloadConverter.ToPayloadContentDescriptor(forgottenPayloadContent);
            if (forgottenPayloadContentDescriptor == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadContentConverter)} of type {_forgettablePayloadConverter.GetType()} returned null when trying to convert {forgottenPayloadContent} to content descriptor. PayloadId: {payloadDescriptor.PayloadId}");
            }

            var updatedDescriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                updatedMetadata,
                forgottenPayloadContentDescriptor);
            
            await _forgettablePayloadStorageWriter.UpdateAsync(updatedDescriptor, cancellationToken)
                .ConfigureAwait(false);

            return updatedDescriptor;
        }
    }
}