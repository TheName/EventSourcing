using EventSourcing.Abstractions.Persistence.DataTransferObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.DataTransferObjects
{
    public class EventStreamEntrySequence_Should
    {
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating(uint sequence)
        {
            EventStreamEntrySequence _ = sequence;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint value)
        {
            EventStreamEntrySequence id1 = value;
            EventStreamEntrySequence id2 = value;
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint value, uint otherValue)
        {
            EventStreamEntrySequence id1 = value;
            EventStreamEntrySequence id2 = otherValue;
            
            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamEntrySequence id)
        {
            var idAsUint = (uint) id;
            
            Assert.Equal(idAsUint.ToString(), id.ToString());
        }
    }
}