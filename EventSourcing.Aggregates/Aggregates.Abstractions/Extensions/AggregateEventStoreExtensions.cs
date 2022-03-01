using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Aggregates.Abstractions.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IAggregateEventStore"/>
    /// </summary>
    public static class AggregateEventStoreExtensions
    {
        /// <summary>
        /// Retrieves aggregate of type <typeparamref name="T"/> and <paramref name="streamId"/>.
        /// </summary>
        /// <param name="aggregateEventStore">
        /// The <see cref="IAggregateEventStore"/>.
        /// </param>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of aggregate.
        /// </typeparam>
        /// <returns>
        /// An instance of <typeparamref name="T"/> with populated event stream with id <paramref name="streamId"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="aggregateEventStore"/> is null.
        /// </exception>
        public static async Task<T> RetrieveAsync<T>(
            this IAggregateEventStore aggregateEventStore,
            EventStreamId streamId,
            CancellationToken cancellationToken)
        {
            if (aggregateEventStore == null)
            {
                throw new ArgumentNullException(nameof(aggregateEventStore));
            }

            return (T) await aggregateEventStore.RetrieveAsync(typeof(T), streamId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}