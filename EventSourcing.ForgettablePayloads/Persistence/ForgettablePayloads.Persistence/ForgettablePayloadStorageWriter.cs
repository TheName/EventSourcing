using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EventSourcing.ForgettablePayloads.Persistence
{
    internal class ForgettablePayloadStorageWriter : IForgettablePayloadStorageWriter
    {
        private readonly IForgettablePayloadStorageRepository _repository;
        private readonly ILogger<ForgettablePayloadStorageWriter> _logger;

        public ForgettablePayloadStorageWriter(
            IForgettablePayloadStorageRepository repository,
            ILogger<ForgettablePayloadStorageWriter> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task InsertAsync(ForgettablePayloadDescriptor forgettablePayloadDescriptor, CancellationToken cancellationToken)
        {
            if (forgettablePayloadDescriptor == null)
            {
                throw new ArgumentNullException(nameof(forgettablePayloadDescriptor));
            }
            
            if (forgettablePayloadDescriptor.PayloadSequence.Value != 0)
            {
                _logger.LogError(
                    "Inserting forgettable payload descriptor with sequence different than 0 is not allowed. Descriptor: \"{Descriptor}\"",
                    forgettablePayloadDescriptor);
                
                throw new InvalidOperationException($"Inserting forgettable payload descriptor with sequence different than 0 is not allowed. Descriptor: \"{forgettablePayloadDescriptor}\"");
            }
            
            if (forgettablePayloadDescriptor.PayloadState != ForgettablePayloadState.Created)
            {
                _logger.LogError(
                    "Inserting forgettable payload descriptor with state different than {AllowedState} is not allowed. Descriptor: \"{Descriptor}\"",
                    ForgettablePayloadState.Created,
                    forgettablePayloadDescriptor);
                
                throw new InvalidOperationException($"Inserting forgettable payload descriptor with state different than {ForgettablePayloadState.Created} is not allowed. Descriptor: \"{forgettablePayloadDescriptor}\"");
            }
            
            var result = await _repository.TryInsertAsync(forgettablePayloadDescriptor, cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError(
                    "Inserting forgettable payload descriptor using repository of type \"{RepositoryType}\" failed. Descriptor: \"{Descriptor}\"",
                    _repository.GetType(),
                    forgettablePayloadDescriptor);
                
                throw new Exception(
                    $"Inserting forgettable payload descriptor using repository of type \"{_repository.GetType()}\" failed. Descriptor: \"{forgettablePayloadDescriptor}\"");
            }
        }

        public async Task UpdateAsync(ForgettablePayloadDescriptor forgettablePayloadDescriptor, CancellationToken cancellationToken)
        {
            if (forgettablePayloadDescriptor == null)
            {
                throw new ArgumentNullException(nameof(forgettablePayloadDescriptor));
            }
            
            var result = await _repository.TryUpdateAsync(forgettablePayloadDescriptor, cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError(
                    "Updating forgettable payload descriptor using repository of type \"{RepositoryType}\" failed. Descriptor: \"{Descriptor}\"",
                    _repository.GetType(),
                    forgettablePayloadDescriptor);
                
                throw new Exception(
                    $"Updating forgettable payload descriptor using repository of type \"{_repository.GetType()}\" failed. Descriptor: \"{forgettablePayloadDescriptor}\"");
            }
        }
    }
}