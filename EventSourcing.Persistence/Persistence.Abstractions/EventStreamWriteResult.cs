namespace EventSourcing.Persistence.Abstractions
{
    /// <summary>
    /// The enum representing event stream write operation result.
    /// </summary>
    public enum EventStreamWriteResult
    {
        /// <summary>
        /// The default value, should not be used at all. Is here as a safety measure in case some property/field of this type would be initialized by default value (0). 
        /// </summary>
        Undefined,
        
        /// <summary>
        /// The write was successful.
        /// </summary>
        Success,
        
        /// <summary>
        /// Event(s) with same sequence(s) is/are already stored in the event source. 
        /// </summary>
        SequenceAlreadyTaken,
        
        /// <summary>
        /// An unknown failure has happened. It is not clear why the write has failed.
        /// </summary>
        UnknownFailure,
        
        /// <summary>
        /// The provided input does not contain any data to insert.
        /// </summary>
        EmptyInput,
    }
}