using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Aggregates.Abstractions
{
    /// <summary>
    /// Publishes and retrieves event stream aggregates.
    /// </summary>
    public interface IAggregateEventStore
    {
        /// <summary>
        /// Publishes provided aggregate.
        /// </summary>
        /// <param name="aggregate">
        /// An event stream aggregate.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the action.
        /// </returns>
        Task PublishAsync(object aggregate, CancellationToken cancellationToken);
        
        /// <summary>
        /// Retrieves an event stream aggregate.
        /// </summary>
        /// <param name="aggregateType">
        /// The <see cref="Type"/> representing the type of an aggregate.
        /// </param>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/> identifying the event stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// An instance of event stream aggregate.
        /// </returns>
        Task<object> RetrieveAsync(Type aggregateType, EventStreamId streamId, CancellationToken cancellationToken);
    }
}