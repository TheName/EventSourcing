using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEntrySequence_Should
    {
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating(uint value)
        {
            EventStreamEntrySequence _ = value;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint value)
        {
            EventStreamEntrySequence sequence1 = value;
            EventStreamEntrySequence sequence2 = value;
            
            Assert.Equal(sequence1, sequence2);
            Assert.True(sequence1 == sequence2);
            Assert.False(sequence1 != sequence2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint value, uint otherValue)
        {
            EventStreamEntrySequence sequence1 = value;
            EventStreamEntrySequence sequence2 = otherValue;
            
            Assert.NotEqual(sequence1, sequence2);
            Assert.False(sequence1 == sequence2);
            Assert.True(sequence1 != sequence2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(uint value)
        {
            EventStreamEntrySequence sequence = value;
            
            Assert.Equal(value.ToString(), sequence.ToString());
        }
    }
}