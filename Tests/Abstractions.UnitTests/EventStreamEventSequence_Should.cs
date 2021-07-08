using EventSourcing.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEventSequence_Should
    {
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating(uint value)
        {
            EventStreamEventSequence _ = value;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint value)
        {
            EventStreamEventSequence sequence1 = value;
            EventStreamEventSequence sequence2 = value;
            
            Assert.Equal(sequence1, sequence2);
            Assert.True(sequence1 == sequence2);
            Assert.False(sequence1 != sequence2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint value, uint otherValue)
        {
            EventStreamEventSequence sequence1 = value;
            EventStreamEventSequence sequence2 = otherValue;
            
            Assert.NotEqual(sequence1, sequence2);
            Assert.False(sequence1 == sequence2);
            Assert.True(sequence1 != sequence2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(uint value)
        {
            EventStreamEventSequence sequence = value;
            
            Assert.Equal(value.ToString(), sequence.ToString());
        }
    }
}