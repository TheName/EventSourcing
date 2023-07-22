using EventSourcing.Abstractions.DependencyInjection;

namespace EventSourcing.Bus.Abstractions
{
    /// <summary>
    /// Builder for EventSourcing Bus
    /// </summary>
    public interface IEventSourcingBusBuilder : IEventSourcingBuilder
    {
        /// <summary>
        /// The <see cref="EventSourcingBusBuilderOptions"/>. 
        /// </summary>
        EventSourcingBusBuilderOptions BusBuilderOptions { get; }
    }
}