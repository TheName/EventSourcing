using System;

namespace EventSourcing.Bus
{
    /// <summary>
    /// The configuration for event sourcing bus handling exception publisher
    /// </summary>
    public interface IEventSourcingBusHandlingExceptionPublisherConfiguration
    {
        /// <summary>
        /// The timeout after which it should be considered that publishing handling exception has failed.
        /// </summary>
        TimeSpan PublishingTimeout { get; }
    }
}
