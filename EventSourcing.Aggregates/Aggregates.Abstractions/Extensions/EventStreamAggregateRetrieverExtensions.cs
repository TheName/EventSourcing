using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Aggregates.Retrievers;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IEventStreamAggregateRetriever"/>
    /// </summary>
    public static class EventStreamAggregateRetrieverExtensions
    {
        /// <summary>
        /// Retrieves aggregate of type <typeparamref name="T"/> and <paramref name="streamId"/>.
        /// </summary>
        /// <param name="aggregateRetriever">
        /// The <see cref="IEventStreamAggregateRetriever"/>.
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
        /// Thrown if <paramref name="aggregateRetriever"/> is null.
        /// </exception>
        public static async Task<T> RetrieveAsync<T>(
            this IEventStreamAggregateRetriever aggregateRetriever,
            EventStreamId streamId,
            CancellationToken cancellationToken)
        {
            if (aggregateRetriever == null)
            {
                throw new ArgumentNullException(nameof(aggregateRetriever));
            }

            return (T) await aggregateRetriever.RetrieveAsync(typeof(T), streamId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
