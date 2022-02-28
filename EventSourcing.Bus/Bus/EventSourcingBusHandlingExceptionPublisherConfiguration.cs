using System;
using EventSourcing.Bus.Abstractions;

namespace EventSourcing.Bus
{
    internal class EventSourcingBusHandlingExceptionPublisherConfiguration : IEventSourcingBusHandlingExceptionPublisherConfiguration
    {
        public TimeSpan PublishingTimeout { get; set; }
    }
}