using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.DependencyInjection
{
    internal class EventSourcingBuilder : IEventSourcingBuilder
    {
        public IServiceCollection Services { get; }

        public EventSourcingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}