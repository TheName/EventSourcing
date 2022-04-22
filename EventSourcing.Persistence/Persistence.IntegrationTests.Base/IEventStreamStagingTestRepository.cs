using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.IntegrationTests.Base
{
    public interface IEventStreamStagingTestRepository
    {
        Task<IReadOnlyList<EventStreamStagingEntryTestReadModel>> SelectAsync(Guid stagingId);

        Task DeleteAllAsync();
    }
}