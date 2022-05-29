using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;

namespace EventSourcing.ForgettablePayloads.Services
{
    internal class ForgettablePayloadDescriptorLoader : IForgettablePayloadDescriptorLoader
    {
        private readonly IForgettablePayloadStorageReader _payloadStorageReader;

        public ForgettablePayloadDescriptorLoader(IForgettablePayloadStorageReader payloadStorageReader)
        {
            _payloadStorageReader = payloadStorageReader ?? throw new ArgumentNullException(nameof(payloadStorageReader));
        }

        public async Task<ForgettablePayloadDescriptor> LoadAsync(
            ForgettablePayloadId forgettablePayloadId,
            CancellationToken cancellationToken)
        {
            if (forgettablePayloadId == null)
            {
                throw new ArgumentNullException(nameof(forgettablePayloadId));
            }
            
            return await _payloadStorageReader
                .ReadAsync(forgettablePayloadId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}