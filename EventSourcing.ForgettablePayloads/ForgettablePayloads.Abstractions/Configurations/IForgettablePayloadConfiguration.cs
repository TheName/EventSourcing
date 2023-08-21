using System;

namespace EventSourcing.ForgettablePayloads.Configurations
{
    /// <summary>
    /// The configuration for forgettable payload event sourcing library
    /// </summary>
    public interface IForgettablePayloadConfiguration
    {
        /// <summary>
        /// The interval at which unclaimed payloads cleanup job should execute.
        /// </summary>
        TimeSpan UnclaimedForgettablePayloadsCleanupJobInterval { get; }

        /// <summary>
        /// The timeout after which (calculated since payload last modified time) unclaimed payloads will be forgotten.
        /// </summary>
        TimeSpan UnclaimedForgettablePayloadsCleanupTimeout { get; }
    }
}
