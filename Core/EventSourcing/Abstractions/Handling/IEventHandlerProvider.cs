using System;
using System.Collections.Generic;

namespace EventSourcing.Abstractions.Handling
{
    internal interface IEventHandlerProvider
    {
        IEnumerable<object> GetHandlersForType(Type eventType);
    }
}