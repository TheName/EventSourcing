using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.IntegrationTests.Base
{
    public interface IEventStreamTestReadRepository
    {
        Task<IReadOnlyList<EventStreamEntryTestReadModel>> SelectAsync(Guid streamId);
    }
}