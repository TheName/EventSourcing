using EventSourcing.ValueObjects;
using Xunit;

namespace Persistence.IntegrationTests.Base
{
    public static class Assertions
    {
        public static void Equal(EventStreamEntry expectedEntry, EventStreamEntryTestReadModel actualEntry)
        {
            Assert.Equal(expectedEntry.StreamId.Value, actualEntry.StreamId);
            Assert.Equal(expectedEntry.EntryId.Value, actualEntry.EntryId);
            Assert.Equal(expectedEntry.EntrySequence.Value, actualEntry.EntrySequence);
            Assert.Equal(expectedEntry.EventDescriptor.EventContent.Value, actualEntry.EventContent);
            Assert.Equal(expectedEntry.EventDescriptor.EventContentSerializationFormat.Value, actualEntry.EventContentSerializationFormat);
            Assert.Equal(expectedEntry.EventDescriptor.EventTypeIdentifier.Value, actualEntry.EventTypeIdentifier);
            Assert.Equal(expectedEntry.EventDescriptor.EventTypeIdentifierFormat.Value, actualEntry.EventTypeIdentifierFormat);
            Assert.Equal(expectedEntry.CausationId.Value, actualEntry.CausationId);
            Assert.Equal(expectedEntry.CreationTime.Value, actualEntry.CreationTime);
            Assert.Equal(expectedEntry.CorrelationId.Value, actualEntry.CorrelationId);
        }
    }
}
