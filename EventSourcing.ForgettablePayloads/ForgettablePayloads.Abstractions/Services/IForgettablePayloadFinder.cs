using System.Collections.Generic;

namespace EventSourcing.ForgettablePayloads.Services
{
    /// <summary>
    /// Finds instances of <see cref="ForgettablePayload"/> in provided event
    /// </summary>
    public interface IForgettablePayloadFinder
    {
        /// <summary>
        /// Finds instances of <see cref="ForgettablePayload"/> in provided event
        /// </summary>
        /// <param name="event">
        /// The event object
        /// </param>
        /// <returns>
        /// The collection of <see cref="ForgettablePayload"/> found in the provided event
        /// </returns>
        IReadOnlyCollection<ForgettablePayload> Find(object @event);
    }
}
