using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Helpers;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Helpers
{
    public class TaskHelpers_Should
    {
        [Theory]
        [AutoMoqData]
        public async Task ThrowAggregateExceptionWithAllThrownExceptions_When_CallingWhenAllWithAggregateException(
            List<Exception> exceptions)
        {
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(() =>
                TaskHelpers.WhenAllWithAggregateException(exceptions.Select(Task.FromException)));
            
            Assert.Equal(exceptions.Count, aggregateException.InnerExceptions.Count);
            Assert.All(exceptions, exception => Assert.Contains(exception, aggregateException.InnerExceptions));
        }
    }
}