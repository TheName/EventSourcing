using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Handling
{
    internal class EventStreamEntryDispatcher : IEventStreamEntryDispatcher
    {
        private readonly IEventStreamEventConverter _eventConverter;
        private readonly IEventHandlerProvider _eventHandlerProvider;
        private readonly IEventHandlingExceptionsHandler _exceptionsHandler;

        public EventStreamEntryDispatcher(
            IEventStreamEventConverter eventConverter,
            IEventHandlerProvider eventHandlerProvider,
            IEventHandlingExceptionsHandler exceptionsHandler)
        {
            _eventConverter = eventConverter ?? throw new ArgumentNullException(nameof(eventConverter));
            _eventHandlerProvider = eventHandlerProvider ?? throw new ArgumentNullException(nameof(eventHandlerProvider));
            _exceptionsHandler = exceptionsHandler ?? throw new ArgumentNullException(nameof(exceptionsHandler));
        }
        
        public async Task DispatchAsync(EventStreamEntry entry, CancellationToken cancellationToken)
        {
            try
            {
                var @event = _eventConverter.FromEventDescriptor(entry.EventDescriptor);
                var eventMetadata = entry.ToEventMetadata();

                EventStreamEntryCorrelationId.Current = eventMetadata.CorrelationId;
                EventStreamEntryCausationId.Current = eventMetadata.EntryId;

                var eventHandlers = _eventHandlerProvider.GetHandlersForType(@event.GetType());
                await Task.WhenAll(
                        eventHandlers
                            .Select(handler => new EventHandlerDecorator(handler))
                            .Select(decorator => decorator.HandleAsync(@event, eventMetadata, cancellationToken)))
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _exceptionsHandler.HandleAsync(entry, e, cancellationToken).ConfigureAwait(false);
            }
        }
        
        private class EventHandlerDecorator
        {
            private readonly object _eventHandler;

            public EventHandlerDecorator(object eventHandler)
            {
                _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            }

            public async Task HandleAsync(
                object @event,
                EventStreamEventMetadata eventMetadata,
                CancellationToken cancellationToken)
            {
                var genericHandlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
                var handleAsyncMethodInfo = genericHandlerType.GetMethod(nameof(HandleAsync));
                if (handleAsyncMethodInfo == null)
                {
                    throw new Exception("Could not get HandleAsync method info.");
                }

                try
                {
                    await (Task) handleAsyncMethodInfo.Invoke(_eventHandler, new[] {@event, eventMetadata, cancellationToken});
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException != null)
                    {
                        throw e.InnerException;
                    }

                    throw;
                }
            }
        }
    }
}