using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Abstractions
{
    /// <summary>
    /// The forgettable payload
    /// </summary>
    public class ForgettablePayload
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private object _payload;
        private ForgettablePayloadCreationTime _payloadCreationTime;
        private ForgettablePayloadDescriptor _payloadDescriptor;

        private IForgettablePayloadDescriptorLoader _payloadLoader;
        private IForgettablePayloadForgettingService _forgettingService;
        private IForgettablePayloadClaimingService _claimingService;
        private IForgettablePayloadContentConverter _payloadConverter;

        private IForgettablePayloadDescriptorLoader PayloadDescriptorLoader
        {
            get => _payloadLoader ?? throw new InvalidOperationException(
                $"Please assign a {nameof(IForgettablePayloadDescriptorLoader)} to this instance. PayloadId: {PayloadId}");
            set => _payloadLoader = value ?? throw new ArgumentNullException(nameof(value));
        }

        private IForgettablePayloadForgettingService ForgettingService
        {
            get => _forgettingService ?? throw new InvalidOperationException(
                $"Please assign a {nameof(IForgettablePayloadForgettingService)} to this instance. PayloadId: {PayloadId}");
            set => _forgettingService = value ?? throw new ArgumentNullException(nameof(value));
        }

        private IForgettablePayloadClaimingService ClaimingService
        {
            get => _claimingService ?? throw new InvalidOperationException(
                $"Please assign a {nameof(IForgettablePayloadClaimingService)} to this instance. PayloadId: {PayloadId}");
            set => _claimingService = value ?? throw new ArgumentNullException(nameof(value));
        }

        private IForgettablePayloadContentConverter Converter
        {
            get => _payloadConverter ?? throw new InvalidOperationException(
                $"Please assign a {nameof(IForgettablePayloadContentConverter)} to this instance. PayloadId: {PayloadId}");
            set => _payloadConverter = value ?? throw new ArgumentNullException(nameof(value));
        }

        private bool WasCreated => _payloadCreationTime != null;

        private bool IsLoaded => _payloadDescriptor != null;

        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayload"/>
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="ForgettablePayload"/>
        /// </returns>
        public static ForgettablePayload<T> CreateNew<T>(T payload)
        {
            if (Equals(payload, default(T)))
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var forgettablePayload = new ForgettablePayload<T>(ForgettablePayloadId.NewForgettablePayloadId())
            {
                _payload = payload,
                _payloadCreationTime = ForgettablePayloadCreationTime.Now()
            };

            return forgettablePayload;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayload"/>
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="ForgettablePayload"/>
        /// </returns>
        public static ForgettablePayload CreateNew(object payload)
        {
            var forgettablePayload = new ForgettablePayload(ForgettablePayloadId.NewForgettablePayloadId())
            {
                _payload = payload ?? throw new ArgumentNullException(nameof(payload)),
                _payloadCreationTime = ForgettablePayloadCreationTime.Now()
            };

            return forgettablePayload;
        }
        
        /// <summary>
        /// The <see cref="ForgettablePayloadId"/>
        /// </summary>
        public ForgettablePayloadId PayloadId { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayload"/>
        /// </summary>
        /// <param name="payloadId">
        /// The <see cref="ForgettablePayloadId"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <see cref="ForgettablePayloadId"/> is null.
        /// </exception>
        public ForgettablePayload(ForgettablePayloadId payloadId)
        {
            PayloadId = payloadId ?? throw new ArgumentNullException(nameof(payloadId));
        }

        /// <summary>
        /// Assigns <see cref="IForgettablePayloadDescriptorLoader"/> to this instance so it could be used later
        /// </summary>
        /// <param name="payloadDescriptorLoader">
        /// The <see cref="IForgettablePayloadDescriptorLoader"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="payloadDescriptorLoader"/> is null.
        /// </exception>
        public void AssignPayloadDescriptorLoaderService(IForgettablePayloadDescriptorLoader payloadDescriptorLoader)
        {
            PayloadDescriptorLoader = payloadDescriptorLoader;
        }

        /// <summary>
        /// Assigns <see cref="IForgettablePayloadForgettingService"/> to this instance so it could be used later
        /// </summary>
        /// <param name="forgettingService">
        /// The <see cref="IForgettablePayloadForgettingService"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="forgettingService"/> is null.
        /// </exception>
        public void AssignForgettingService(IForgettablePayloadForgettingService forgettingService)
        {
            ForgettingService = forgettingService;
        }

        /// <summary>
        /// Assigns <see cref="IForgettablePayloadClaimingService"/> to this instance so it could be used later
        /// </summary>
        /// <param name="claimingService">
        /// The <see cref="IForgettablePayloadClaimingService"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="claimingService"/> is null.
        /// </exception>
        public void AssignClaimingService(IForgettablePayloadClaimingService claimingService)
        {
            ClaimingService = claimingService;
        }

        /// <summary>
        /// Assigns <see cref="IForgettablePayloadContentConverter"/> to this instance so it could be used later
        /// </summary>
        /// <param name="payloadContentConverter">
        /// The <see cref="IForgettablePayloadContentConverter"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="payloadContentConverter"/> is null.
        /// </exception>
        public void AssignContentConverter(IForgettablePayloadContentConverter payloadContentConverter)
        {
            Converter = payloadContentConverter;
        }

        /// <summary>
        /// Returns true if this payload is already forgotten, false otherwise
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// True if this payload is already forgotten, false otherwise
        /// </returns>
        public async Task<bool> IsForgottenAsync(CancellationToken cancellationToken)
        {
            if (WasCreated)
            {
                // if this instance was created it cannot have been forgotten yet.
                return false;
            }
            
            await LoadIfNotLoadedAsync(cancellationToken).ConfigureAwait(false);
            
            return _payloadDescriptor.PayloadState == ForgettablePayloadState.Forgotten;
        }

        /// <summary>
        /// Returns true if this payload is already claimed, false otherwise
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// True if this payload is already claimed, false otherwise
        /// </returns>
        public async Task<bool> IsClaimedAsync(CancellationToken cancellationToken)
        {
            if (WasCreated)
            {
                // if this instance was created it cannot have been claimed yet.
                return false;
            }
            
            await LoadIfNotLoadedAsync(cancellationToken).ConfigureAwait(false);
            
            return _payloadDescriptor.PayloadState == ForgettablePayloadState.CreatedAndClaimed;
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
        public async Task<object> GetPayloadAsync(CancellationToken cancellationToken)
        {
            if (WasCreated)
            {
                return _payload;
            }
            
            await LoadIfNotLoadedAsync(cancellationToken).ConfigureAwait(false);

            if (_payload != null)
            {
                return _payload;
            }
            
            var converter = Converter;
            var payload = converter.FromPayloadContentDescriptor(_payloadDescriptor.ToContentDescriptor());

            _payload = payload ?? throw new InvalidOperationException(
                $"{nameof(IForgettablePayloadContentConverter)} of type {converter.GetType().Name} returned null when trying to convert {_payloadDescriptor.ToContentDescriptor()}. PayloadId: {PayloadId}");

            return _payload;
        }

        /// <summary>
        /// Creates a new metadata object for provided stream id and entry id
        /// </summary>
        /// <param name="eventStreamId">
        /// The <see cref="EventStreamId"/>
        /// </param>
        /// <param name="eventStreamEntryId">
        /// The <see cref="EventStreamEntryId"/>
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadMetadata"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this instance has not been created but loaded (and thus already stored and assigned to other stream id and entry id)
        /// </exception>
        public ForgettablePayloadMetadata CreateMetadataForEventStreamIdAndEntryId(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId)
        {
            if (!WasCreated)
            {
                throw new InvalidOperationException(
                    $"This instance of {nameof(ForgettablePayload)} was not created; cannot create a new metadata for a loaded one. Payload Id: {PayloadId}");
            }

            return new ForgettablePayloadMetadata(
                eventStreamId,
                eventStreamEntryId,
                PayloadId,
                ForgettablePayloadState.Created, 
                _payloadCreationTime,
                new ForgettablePayloadLastModifiedTime(_payloadCreationTime.Value),
                new ForgettablePayloadSequence(0));
        }

        /// <summary>
        /// Forgets the original payload
        /// </summary>
        /// <param name="forgettingPayloadReason">
        /// The <see cref="ForgettingPayloadReason"/>
        /// </param>
        /// <param name="forgettingPayloadRequestedBy">
        /// The <see cref="ForgettingPayloadRequestedBy"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        public async Task ForgetAsync(
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            CancellationToken cancellationToken)
        {
            if (forgettingPayloadReason == null)
            {
                throw new ArgumentNullException(nameof(forgettingPayloadReason));
            }

            if (forgettingPayloadRequestedBy == null)
            {
                throw new ArgumentNullException(nameof(forgettingPayloadRequestedBy));
            }
            
            if (await IsForgottenAsync(cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            var descriptor = await GetDescriptorAsync(cancellationToken).ConfigureAwait(false);
            
            var forgettingService = ForgettingService;
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var forgottenDescriptor = await forgettingService.ForgetAsync(
                        descriptor,
                        forgettingPayloadReason,
                        forgettingPayloadRequestedBy,
                        cancellationToken)
                    .ConfigureAwait(false);

                ReloadDescriptor(forgottenDescriptor);
            }
            finally
            {
                _semaphore.Release();
            }
            
            var isForgotten = await IsForgottenAsync(cancellationToken).ConfigureAwait(false);
            if (!isForgotten)
            {
                throw new InvalidOperationException(
                    $"This instance of {nameof(ForgettablePayload)} does not have forgotten state assigned to it after using forgetting service of type {forgettingService.GetType().Name}. PayloadId: {PayloadId}");
            }
        }

        /// <summary>
        /// Claims this forgettable payload and updates this instance's metadata
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        public async Task ClaimAsync(CancellationToken cancellationToken)
        {
            if (await IsClaimedAsync(cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            var descriptor = await GetDescriptorAsync(cancellationToken).ConfigureAwait(false);

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            var claimingService = ClaimingService;
            try
            {
                var claimedDescriptor = await claimingService.ClaimAsync(
                        descriptor,
                        cancellationToken)
                    .ConfigureAwait(false);

                ReloadDescriptor(claimedDescriptor);
            }
            finally
            {
                _semaphore.Release();
            }
            
            var isClaimed = await IsClaimedAsync(cancellationToken).ConfigureAwait(false);
            if (!isClaimed)
            {
                throw new InvalidOperationException(
                    $"This instance of {nameof(ForgettablePayload)} does not have claimed state assigned to it after using claiming service of type {claimingService.GetType().Name}. PayloadId: {PayloadId}");
            }
        }
        
        private async Task<ForgettablePayloadDescriptor> GetDescriptorAsync(CancellationToken cancellationToken)
        {
            await LoadIfNotLoadedAsync(cancellationToken).ConfigureAwait(false);

            return _payloadDescriptor;
        }

        private async Task LoadIfNotLoadedAsync(CancellationToken cancellationToken)
        {
            if (IsLoaded)
            {
                return;
            }

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await LoadAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task LoadAsync(CancellationToken cancellationToken)
        {
            if (WasCreated)
            {
                throw new InvalidOperationException(
                    $"This instance of {nameof(ForgettablePayload)} was created as new. Cannot load it.");
            }

            var loader = PayloadDescriptorLoader;
            var descriptor = await loader.LoadAsync(PayloadId, cancellationToken).ConfigureAwait(false);
            ReloadDescriptor(descriptor);

            if (!IsLoaded)
            {
                throw new InvalidOperationException(
                    $"{nameof(ForgettablePayload)} has not been loaded after using loader of type {loader.GetType().Name}. PayloadId: {PayloadId}");
            }
        }

        private void ReloadDescriptor(ForgettablePayloadDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (descriptor.PayloadId != PayloadId)
            {
                throw new InvalidOperationException(
                    $"Cannot reload {nameof(ForgettablePayload)} with {nameof(PayloadId)} \"{PayloadId}\" using {nameof(ForgettablePayloadDescriptor)} with {nameof(PayloadId)} \"{descriptor.PayloadId}\"");
            }

            _payloadDescriptor = descriptor;
            _payload = null;
            _payloadCreationTime = null;
        }
    }
}