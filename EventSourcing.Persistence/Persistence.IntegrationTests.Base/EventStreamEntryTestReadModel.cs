using System;

namespace Persistence.IntegrationTests.Base
{
    public class EventStreamEntryTestReadModel
    {
        public Guid StreamId { get; set; }
        public uint EntrySequence { get; set; }
        public Guid EntryId { get; set; }
        public string EventContent { get; set; }
        public string EventContentSerializationFormat { get; set; }
        public string EventTypeIdentifier { get; set; }
        public string EventTypeIdentifierFormat { get; set; }
        public Guid CausationId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid CorrelationId { get; set; }
    }
}