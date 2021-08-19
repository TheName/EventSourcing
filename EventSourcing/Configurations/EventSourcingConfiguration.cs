using EventSourcing.Abstractions.Configurations;

namespace EventSourcing.Configurations
{
    internal class EventSourcingConfiguration : IEventSourcingConfiguration
    {
        public string BoundedContext { get; set; }
    }
}