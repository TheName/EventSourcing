using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Persistence.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream staged entries value object.
    /// </summary>
    public class EventStreamStagedEntries
    {
        /// <summary>
        /// The <see cref="EventStreamStagingId"/>
        /// </summary>
        public EventStreamStagingId StagingId { get; }

        /// <summary>
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>
        /// </summary>
        public EventStreamStagedEntriesStagingTime StagingTime { get; }

        /// <summary>
        /// The <see cref="EventStreamEntries"/>
        /// </summary>
        public EventStreamEntries Entries { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamStagedEntries"/> class.
        /// </summary>
        /// <param name="stagingId">
        /// The <see cref="EventStreamStagingId"/>
        /// </param>
        /// <param name="stagingTime">
        /// The <see cref="EventStreamStagedEntriesStagingTime"/>
        /// </param>
        /// <param name="entries">
        /// The <see cref="EventStreamEntries"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="stagingId"/> or <paramref name="entries"/> is null.
        /// </exception>
        public EventStreamStagedEntries(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            StagingId = stagingId ?? throw new ArgumentNullException(nameof(stagingId));
            StagingTime = stagingTime ?? throw new ArgumentNullException(nameof(stagingTime));
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="stagedEntries">
        /// The <see cref="EventStreamStagedEntries"/>.
        /// </param>
        /// <param name="otherStagedEntries">
        /// The <see cref="EventStreamStagedEntries"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="stagedEntries"/> and <paramref name="otherStagedEntries"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamStagedEntries stagedEntries, EventStreamStagedEntries otherStagedEntries) =>
            Equals(stagedEntries, otherStagedEntries);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="stagedEntries">
        /// The <see cref="EventStreamStagedEntries"/>.
        /// </param>
        /// <param name="otherStagedEntries">
        /// The <see cref="EventStreamStagedEntries"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="stagedEntries"/> and <paramref name="otherStagedEntries"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamStagedEntries stagedEntries, EventStreamStagedEntries otherStagedEntries) =>
            !(stagedEntries == otherStagedEntries);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamStagedEntries other &&
            other.GetPropertiesForHashCode().SequenceEqual(GetPropertiesForHashCode());

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Select(o => o.GetHashCode())
                    .Where(i => i != 0)
                    .Aggregate(17, (current, hashCode) => current * 23 * hashCode);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Event Stream Staging ID: {StagingId}, Staging Time: {StagingTime}, Entries: {Entries}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StagingId;
            yield return StagingTime;
            yield return Entries;
        }
    }
}