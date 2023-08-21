namespace EventSourcing.Hooks
{
    /// <summary>
    /// A hook invoked after event descriptor deserialization
    /// </summary>
    public interface IEventStreamEventDescriptorPostDeserializationHook
    {
        /// <summary>
        /// Method invoked after event descriptor deserialization
        /// </summary>
        /// <param name="event">
        /// The deserialized event object
        /// </param>
        void PostEventDeserializationHook(object @event);
    }
}
