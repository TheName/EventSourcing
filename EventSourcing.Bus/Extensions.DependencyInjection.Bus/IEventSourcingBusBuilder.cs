namespace EventSourcing.Extensions.DependencyInjection.Bus
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