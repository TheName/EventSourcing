﻿using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Abstractions.DependencyInjection
{
    /// <summary>
    /// Builder for EventSourcing
    /// </summary>
    public interface IEventSourcingBuilder
    {
        /// <summary>
        /// The <see cref="IServiceCollection"/>
        /// </summary>
        IServiceCollection Services { get; }
    }
}