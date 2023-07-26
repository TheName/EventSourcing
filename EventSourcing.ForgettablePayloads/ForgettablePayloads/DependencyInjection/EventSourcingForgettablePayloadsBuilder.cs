using System;
using EventSourcing.Abstractions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.DependencyInjection
{
    internal class EventSourcingForgettablePayloadsBuilder : IEventSourcingForgettablePayloadsBuilder
    {
        private readonly IEventSourcingBuilder _eventSourcingBuilder;
        
        public IServiceCollection Services => _eventSourcingBuilder.Services;

        public EventSourcingForgettablePayloadsBuilder(IEventSourcingBuilder eventSourcingBuilder)
        {
            _eventSourcingBuilder = eventSourcingBuilder ?? throw new ArgumentNullException(nameof(eventSourcingBuilder));
        }
    }
}