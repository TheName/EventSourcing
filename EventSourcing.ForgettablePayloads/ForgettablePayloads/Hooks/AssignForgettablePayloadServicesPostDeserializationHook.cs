using System;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.Hooks;

namespace EventSourcing.ForgettablePayloads.Hooks
{
    internal class AssignForgettablePayloadServicesPostDeserializationHook : IEventStreamEventDescriptorPostDeserializationHook
    {
        private readonly IForgettablePayloadFinder _forgettablePayloadFinder;
        private readonly IForgettablePayloadDescriptorLoader _forgettablePayloadLoader;
        private readonly IForgettablePayloadForgettingService _forgettingService;
        private readonly IForgettablePayloadClaimingService _claimingService;
        private readonly IForgettablePayloadContentConverter _converter;

        public AssignForgettablePayloadServicesPostDeserializationHook(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadClaimingService claimingService,
            IForgettablePayloadContentConverter converter)
        {
            _forgettablePayloadFinder = forgettablePayloadFinder ?? throw new ArgumentNullException(nameof(forgettablePayloadFinder));
            _forgettablePayloadLoader = forgettablePayloadLoader ?? throw new ArgumentNullException(nameof(forgettablePayloadLoader));
            _forgettingService = forgettingService ?? throw new ArgumentNullException(nameof(forgettingService));
            _claimingService = claimingService ?? throw new ArgumentNullException(nameof(claimingService));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public void PostEventDeserializationHook(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            
            var forgettablePayloadEntities = _forgettablePayloadFinder.Find(@event);
            if (forgettablePayloadEntities == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadFinder)} of type {_forgettablePayloadFinder.GetType()} returned null when trying to find forgettable payloads for event type {@event.GetType()} and instance {@event}");
            }

            foreach (var forgettablePayloadEntity in forgettablePayloadEntities)
            {
                forgettablePayloadEntity.AssignPayloadDescriptorLoaderService(_forgettablePayloadLoader);
                forgettablePayloadEntity.AssignForgettingService(_forgettingService);
                forgettablePayloadEntity.AssignClaimingService(_claimingService);
                forgettablePayloadEntity.AssignContentConverter(_converter);
            }
        }
    }
}