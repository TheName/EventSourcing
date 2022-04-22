using System;
using EventSourcing.Abstractions.Configurations;

namespace EventSourcing.Configurations
{
    internal class EventSourcingConfiguration : IEventSourcingConfiguration
    {
        public string BoundedContext { get; set; }
        
        public TimeSpan ReconciliationJobInterval { get; set; }
        
        public TimeSpan ReconciliationJobGracePeriodAfterStagingTime { get; set; }
    }
}