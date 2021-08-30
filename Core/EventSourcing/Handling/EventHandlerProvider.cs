using System;
using System.Collections.Generic;
using EventSourcing.Abstractions.Handling;

namespace EventSourcing.Handling
{
    internal class EventHandlerProvider : IEventHandlerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public EventHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public IEnumerable<object> GetHandlersForType(Type eventType)
        {
            if (eventType == null)
            {
                throw new ArgumentNullException(nameof(eventType));
            }
            
            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            return (IEnumerable<object>) _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(eventHandlerType));
        }
    }
}