namespace EventSourcing.Bus
{
    /// <summary>
    /// Options related to event sourcing builder for bus registrations
    /// </summary>
    public class EventSourcingBusBuilderOptions
    {
        /// <summary>
        /// Defines whether event source entry handler should be registered and started.
        /// Defaults to true.
        /// </summary>
        public bool WithConsumer { get; }

        /// <summary>
        /// EventSourcingBusBuilderOptions constructor
        /// </summary>
        /// <param name="withConsumer">
        /// The <see cref="WithConsumer"/>
        /// </param>
        public EventSourcingBusBuilderOptions(bool withConsumer)
        {
            WithConsumer = withConsumer;
        }

        /// <summary>
        /// Default EventSourcingBusBuilderOptions constructor
        /// </summary>
        public EventSourcingBusBuilderOptions() : this(true)
        {
        }
    }
}
