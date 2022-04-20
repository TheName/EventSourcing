using System;

namespace Persistence.IntegrationTests.Base
{
    public class EventStreamStagingEntryTestReadModel : EventStreamEntryTestReadModel
    {
        public Guid StagingId { get; set; }
    }
}