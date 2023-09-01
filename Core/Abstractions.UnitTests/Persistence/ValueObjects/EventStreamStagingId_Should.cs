using System;
using EventSourcing.Persistence.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.ValueObjects
{
    public class EventStreamStagingId_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithEmptyGuid()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamStagingId) Guid.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyGuid(Guid id)
        {
            EventStreamStagingId _ = id;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(Guid value)
        {
            EventStreamStagingId id1 = value;
            EventStreamStagingId id2 = value;

            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(Guid value, Guid otherValue)
        {
            EventStreamStagingId id1 = value;
            EventStreamStagingId id2 = otherValue;

            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamStagingId id)
        {
            var idAsGuid = (Guid) id;

            Assert.Equal(idAsGuid.ToString(), id.ToString());
        }
    }
}
