using System;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.Serialization.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Serialization.DependencyInjection
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