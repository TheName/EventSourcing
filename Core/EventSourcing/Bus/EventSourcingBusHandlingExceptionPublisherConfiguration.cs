using System;

namespace EventSourcing.Bus
{
    internal class EventSourcingBusHandlingExceptionPublisherConfiguration : IEventSourcingBusHandlingExceptionPublisherConfiguration
    {
        public TimeSpan PublishingTimeout { get; set; }
    }
}