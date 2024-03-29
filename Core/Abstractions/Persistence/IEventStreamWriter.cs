﻿using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Enums;
using EventSourcing.ValueObjects;

namespace EventSourcing.Persistence
{
    /// <summary>
    /// The event stream writer used to modify event source.
    /// </summary>
    public interface IEventStreamWriter
    {
        /// <summary>
        /// Writes <paramref name="eventStreamEntries"/> and returns a single <see cref="EventStreamWriteResult"/> that represents the writing result.
        /// </summary>
        /// <param name="eventStreamEntries">
        /// The <see cref="EventStreamEntries"/> that should be stored.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamWriteResult"/> that represents the writing result.
        /// </returns>
        Task<EventStreamWriteResult> WriteAsync(
            EventStreamEntries eventStreamEntries,
            CancellationToken cancellationToken);
    }
}
