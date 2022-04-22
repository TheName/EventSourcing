using System;

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
        
        /// <summary>
        /// The interval at which reconciliation job should execute.
        /// </summary>
        TimeSpan ReconciliationJobInterval { get; }
        
        /// <summary>
        /// The grace period that should pass after staging time before reconciliation for staged entries starts
        /// </summary>
        TimeSpan ReconciliationJobGracePeriodAfterStagingTime { get; }
    }
}