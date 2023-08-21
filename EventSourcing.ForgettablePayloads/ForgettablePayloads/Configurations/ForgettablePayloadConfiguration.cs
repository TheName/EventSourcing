using System;

namespace EventSourcing.ForgettablePayloads.Configurations
{
    internal class ForgettablePayloadConfiguration : IForgettablePayloadConfiguration
    {
        public TimeSpan UnclaimedForgettablePayloadsCleanupJobInterval { get; set; }
        
        public TimeSpan UnclaimedForgettablePayloadsCleanupTimeout { get; set; }
    }
}