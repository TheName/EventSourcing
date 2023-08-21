using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates.Conversion
{
    /// <summary>
    /// Converts aggregates to <see cref="PublishableEventStream"/>.
    /// </summary>
    public interface IEventStreamAggregateConverter
    {
        /// <summary>
        /// Converts <paramref name="aggregate"/> to <see cref="PublishableEventStream"/>.
        /// </summary>
        /// <param name="aggregate">
        /// An object representing an aggregate that should be converted to <see cref="PublishableEventStream"/>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="PublishableEventStream"/> converted from provided <paramref name="aggregate"/>.
        /// </returns>
        PublishableEventStream ToPublishableEventStream(object aggregate);
    }
}
