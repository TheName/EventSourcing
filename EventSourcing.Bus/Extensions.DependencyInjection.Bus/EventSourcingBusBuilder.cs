using System;
using EventSourcing.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Extensions.DependencyInjection.Bus
{
    internal class EventSourcingBusBuilder : IEventSourcingBusBuilder
    {
        private readonly IEventSourcingBuilder _eventSourcingBuilder;
        public IServiceCollection Services => _eventSourcingBuilder.Services;
        
        public EventSourcingBusBuilderOptions BusBuilderOptions { get; }

        public EventSourcingBusBuilder(
            IEventSourcingBuilder eventSourcingBuilder,
            EventSourcingBusBuilderOptions busBuilderOptions)
        {
            _eventSourcingBuilder = eventSourcingBuilder ?? throw new ArgumentNullException(nameof(eventSourcingBuilder));
            BusBuilderOptions = busBuilderOptions ?? throw new ArgumentNullException(nameof(busBuilderOptions));
        }
    }
}