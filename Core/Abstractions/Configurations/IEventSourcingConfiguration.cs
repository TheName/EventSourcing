namespace EventSourcing.Abstractions.Configurations
{
    /// <summary>
    /// The configuration for event sourcing library
    /// </summary>
    public interface IEventSourcingConfiguration
    {
        /// <summary>
        /// The bounded context
        /// </summary>
        string BoundedContext { get; }
    }
}