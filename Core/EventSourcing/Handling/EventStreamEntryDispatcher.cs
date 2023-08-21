using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Conversion;
using EventSourcing.Exceptions;
using EventSourcing.Helpers;
using EventSourcing.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Handling
{
    internal class EventStreamEntryDispatcher : IEventStreamEntryDispatcher
    {
        private readonly IEventStreamEventConverter _eventConverter;
        private readonly IEventHandlerProvider _eventHandlerProvider;
        private readonly IEventHandlingExceptionsHandler _handlingExceptionsHandler;
        private readonly ILogger<EventStreamEntryDispatcher> _logger;

        public EventStreamEntryDispatcher(
            IEventStreamEventConverter eventConverter,
            IEventHandlerProvider eventHandlerProvider,
            IEventHandlingExceptionsHandler handlingExceptionsHandler,
            ILogger<EventStreamEntryDispatcher> logger)
        {
            _eventConverter = eventConverter ?? throw new ArgumentNullException(nameof(eventConverter));
            _eventHandlerProvider = eventHandlerProvider ?? throw new ArgumentNullException(nameof(eventHandlerProvider));
            _handlingExceptionsHandler = handlingExceptionsHandler ?? throw new ArgumentNullException(nameof(handlingExceptionsHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task DispatchAsync(EventStreamEntry entry, CancellationToken cancellationToken)
        {
            try
            {
                EventStreamEntryCorrelationId.Current = entry.CorrelationId;
                EventStreamEntryCausationId.Current = entry.EntryId;
                
                var @event = ConvertToEvent(entry.EventDescriptor);
                var eventMetadata = entry.ToEventMetadata();

                var decoratedEventHandlers = GetDecoratedHandlersForEventType(@event.GetType());
                var decoratedEventHandlingTasks = decoratedEventHandlers
                    .Select(decorator => decorator.HandleAsync(@event, eventMetadata, cancellationToken));

                await TaskHelpers.WhenAllWithAggregateException(decoratedEventHandlingTasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "There was an error when trying to handle event stream entry {@EventStreamEntry}",
                    entry);

                var entryHandlingException = GetEventStreamEntryHandlingException(entry, e);
                await _handlingExceptionsHandler.HandleAsync(entryHandlingException, cancellationToken).ConfigureAwait(false);
            }
        }

        private object ConvertToEvent(EventStreamEventDescriptor eventDescriptor)
        {
            try
            {
                return _eventConverter.FromEventDescriptor(eventDescriptor);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "There was an error when trying to convert event descriptor {@EventDescriptor} to an event object",
                    eventDescriptor);
                
                throw;
            }
        }

        private IReadOnlyList<EventHandlerDecorator> GetDecoratedHandlersForEventType(Type eventType)
        {
            var eventHandlers = GetHandlersForEventType(eventType);
            return DecorateEventHandlers(eventType, eventHandlers, _logger);
        }

        private IReadOnlyList<object> GetHandlersForEventType(Type eventType)
        {
            try
            {
                return _eventHandlerProvider.GetHandlersForType(eventType).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "There was an error when trying to get event handlers for event type {EventType}",
                    eventType);
                
                throw;
            }
        }

        private IReadOnlyList<EventHandlerDecorator> DecorateEventHandlers(
            Type eventType,
            IEnumerable<object> eventHandlers,
            ILogger logger)
        {
            try
            {
                return eventHandlers
                    .Select(handler => new EventHandlerDecorator(handler, logger))
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "There was an error when trying to decorate event handlers for event type {EventType}",
                    eventType);
                
                throw;
            }
        }

        private EventStreamEntryHandlingException GetEventStreamEntryHandlingException(
            EventStreamEntry entry,
            Exception exception)
        {
            try
            {
                return EventStreamEntryHandlingException.New(
                    entry,
                    EventStreamEntryHandlingTime.Now(),
                    exception);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "There was an error when trying to create entry handling exception {@EventStreamEntry}",
                    entry);
                
                throw;
            }
        }

        private class EventHandlerDecorator
        {
            private readonly object _eventHandler;
            private readonly ILogger _logger;

            public EventHandlerDecorator(object eventHandler, ILogger logger)
            {
                _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

                _logger.LogInformation(
                    "Invoking handler {HandlerName} in order to handle event {Event} with metadata {Metadata}",
                    _eventHandler.GetType(),
                    @event,
                    eventMetadata);
                
                try
                {
                    await (Task) handleAsyncMethodInfo.Invoke(_eventHandler, new[] {@event, eventMetadata, cancellationToken});
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException != null)
                    {
                        _logger.LogError(
                            e.InnerException,
                            "Handler {HandlerName} failed to handle event {Event} with metadata {Metadata}",
                            _eventHandler.GetType(),
                            @event,
                            eventMetadata);
                        
                        throw e.InnerException;
                    }

                    _logger.LogError(
                        e,
                        "Handler {HandlerName} failed to handle event {Event} with metadata {Metadata}",
                        _eventHandler.GetType(),
                        @event,
                        eventMetadata);
                    
                    throw;
                }
                
                _logger.LogInformation(
                    "Handler {HandlerName} has successfully handled event {Event} with metadata {Metadata}",
                    _eventHandler.GetType(),
                    @event,
                    eventMetadata);
            }
        }
    }
}
