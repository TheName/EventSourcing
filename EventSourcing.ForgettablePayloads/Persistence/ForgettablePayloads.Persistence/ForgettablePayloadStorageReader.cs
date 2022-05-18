using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;

namespace EventSourcing.ForgettablePayloads.Persistence
{
    internal class ForgettablePayloadStorageReader : IForgettablePayloadStorageReader
    {
        private readonly IForgettablePayloadStorageRepository _repository;

        public ForgettablePayloadStorageReader(IForgettablePayloadStorageRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(
            EventStreamId eventStreamId,
            CancellationToken cancellationToken)
        {
            if (eventStreamId == null)
            {
                throw new ArgumentNullException(nameof(eventStreamId));
            }
            
            return await _repository.ReadAsync(eventStreamId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            CancellationToken cancellationToken)
        {
            if (eventStreamId == null)
            {
                throw new ArgumentNullException(nameof(eventStreamId));
            }

            if (eventStreamEntryId == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEntryId));
            }
            
            return await _repository.ReadAsync(
                    eventStreamId,
                    eventStreamEntryId,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<ForgettablePayloadDescriptor> ReadAsync(
            ForgettablePayloadId forgettablePayloadId,
            CancellationToken cancellationToken)
        {
            if (forgettablePayloadId == null)
            {
                throw new ArgumentNullException(nameof(forgettablePayloadId));
            }
            
            return await _repository.ReadAsync(
                    forgettablePayloadId,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadUnclaimedAsync(CancellationToken cancellationToken)
        {
            return await _repository.ReadAsync(ForgettablePayloadState.Created, cancellationToken).ConfigureAwait(false);
        }
    }
}