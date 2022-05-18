using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Helpers;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;

namespace EventSourcing.ForgettablePayloads.Services
{
    internal class ForgettablePayloadEventStreamForgettingService : IForgettablePayloadEventStreamForgettingService
    {
        private readonly IForgettablePayloadStorageReader _forgettablePayloadStorageReader;
        private readonly IForgettablePayloadForgettingService _forgettingService;

        public ForgettablePayloadEventStreamForgettingService(
            IForgettablePayloadStorageReader forgettablePayloadStorageReader,
            IForgettablePayloadForgettingService forgettingService)
        {
            _forgettablePayloadStorageReader = forgettablePayloadStorageReader ?? throw new ArgumentNullException(nameof(forgettablePayloadStorageReader));
            _forgettingService = forgettingService ?? throw new ArgumentNullException(nameof(forgettingService));
        }
        
        public async Task ForgetAllForgettablePayloadsFromStreamAsync(
            EventStreamId streamId,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            CancellationToken cancellationToken)
        {
            if (streamId == null)
            {
                throw new ArgumentNullException(nameof(streamId));
            }

            if (forgettingPayloadReason == null)
            {
                throw new ArgumentNullException(nameof(forgettingPayloadReason));
            }

            if (forgettingPayloadRequestedBy == null)
            {
                throw new ArgumentNullException(nameof(forgettingPayloadRequestedBy));
            }
            
            var forgettablePayloadsDescriptors = await _forgettablePayloadStorageReader.ReadAsync(streamId, cancellationToken)
                .ConfigureAwait(false);
            
            if (forgettablePayloadsDescriptors == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadStorageReader)} of type {_forgettablePayloadStorageReader.GetType()} returned null when trying to read forgettable payload descriptors for event stream id {streamId}");
            }

            await TaskHelpers.WhenAllWithAggregateException(
                    forgettablePayloadsDescriptors
                        .Select(descriptor => _forgettingService.ForgetAsync(
                            descriptor,
                            forgettingPayloadReason,
                            forgettingPayloadRequestedBy,
                            cancellationToken)))
                .ConfigureAwait(false);
        }
    }
}