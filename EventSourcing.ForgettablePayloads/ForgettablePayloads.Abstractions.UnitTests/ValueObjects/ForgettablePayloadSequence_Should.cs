using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadSequence_Should
    {
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating(uint value)
        {
            ForgettablePayloadSequence _ = value;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint value)
        {
            ForgettablePayloadSequence sequence1 = value;
            ForgettablePayloadSequence sequence2 = value;
            
            Assert.Equal(sequence1, sequence2);
            Assert.True(sequence1 == sequence2);
            Assert.False(sequence1 != sequence2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint value, uint otherValue)
        {
            ForgettablePayloadSequence sequence1 = value;
            ForgettablePayloadSequence sequence2 = otherValue;
            
            Assert.NotEqual(sequence1, sequence2);
            Assert.False(sequence1 == sequence2);
            Assert.True(sequence1 != sequence2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(uint value)
        {
            ForgettablePayloadSequence sequence = value;
            
            Assert.Equal(value.ToString(), sequence.ToString());
        }
    }
}