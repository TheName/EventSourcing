using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Helpers;

namespace EventSourcing.ForgettablePayloads.Handlers
{
    internal class ForgettablePayloadClaimingHandler : IEventHandler<object>
    {
        private readonly IForgettablePayloadFinder _forgettablePayloadFinder;

        public ForgettablePayloadClaimingHandler(IForgettablePayloadFinder forgettablePayloadFinder)
        {
            _forgettablePayloadFinder = forgettablePayloadFinder ?? throw new ArgumentNullException(nameof(forgettablePayloadFinder));
        }

        public async Task HandleAsync(
            object @event,
            EventStreamEventMetadata eventMetadata,
            CancellationToken cancellationToken)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (eventMetadata == null)
            {
                throw new ArgumentNullException(nameof(eventMetadata));
            }
            
            var forgettablePayloadEntities = _forgettablePayloadFinder.Find(@event);
            if (forgettablePayloadEntities == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadFinder)} of type {_forgettablePayloadFinder.GetType()} returned null when trying to find forgettable payloads for event type {@event.GetType()} and instance {@event}");
            }

            await TaskHelpers.WhenAllWithAggregateException(forgettablePayloadEntities
                    .Select(payload => payload.ClaimAsync(cancellationToken)))
                .ConfigureAwait(false);
        }
    }
}