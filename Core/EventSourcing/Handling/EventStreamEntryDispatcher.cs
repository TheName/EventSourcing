using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;
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
                var @event = ConvertToEvent(entry.EventDescriptor);
                var eventMetadata = entry.ToEventMetadata();

                EventStreamEntryCorrelationId.Current = eventMetadata.CorrelationId;
                EventStreamEntryCausationId.Current = eventMetadata.EntryId;

                var decoratedEventHandlers = GetDecoratedHandlersForEventType(@event.GetType());
                var decoratedEventHandlingTasks = decoratedEventHandlers
                    .Select(decorator => decorator.HandleAsync(@event, eventMetadata, cancellationToken));

                await WhenAll(decoratedEventHandlingTasks).ConfigureAwait(false);
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
            return DecorateEventHandlers(eventType, eventHandlers);
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

        private IReadOnlyList<EventHandlerDecorator> DecorateEventHandlers(Type eventType, IEnumerable<object> eventHandlers)
        {
            try
            {
                return eventHandlers
                    .Select(handler => new EventHandlerDecorator(handler))
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

        /// <summary>
        /// Throws an aggregate exception instead of the first one
        /// </summary>
        private static async Task WhenAll(IEnumerable<Task> tasks)
        {
            var whenAllTask = Task.WhenAll(tasks);

            try
            {
                await whenAllTask.ConfigureAwait(false);
            }
            catch
            {
                if (whenAllTask.Exception != null)
                {
                    // throw aggregate exception instead of the first one
                    throw whenAllTask.Exception;
                }

                throw;
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