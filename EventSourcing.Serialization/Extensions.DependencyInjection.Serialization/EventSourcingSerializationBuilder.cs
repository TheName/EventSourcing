using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Extensions.DependencyInjection.Serialization
{
    internal class EventSourcingSerializationBuilder : IEventSourcingSerializationBuilder
    {
        private readonly IEventSourcingBuilder _eventSourcingBuilder;
        public IServiceCollection Services => _eventSourcingBuilder.Services;

        public EventSourcingSerializationBuilder(IEventSourcingBuilder eventSourcingBuilder)
        {
            _eventSourcingBuilder = eventSourcingBuilder ?? throw new ArgumentNullException(nameof(eventSourcingBuilder));
        }
    }
}