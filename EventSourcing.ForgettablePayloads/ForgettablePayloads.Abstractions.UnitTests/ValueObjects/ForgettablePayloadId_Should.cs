using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadId_Should
    {
        [Fact]
        public void ReturnRandomForgettablePayloadId_When_CallingNewForgettablePayloadId()
        {
            var streamId = ForgettablePayloadId.NewForgettablePayloadId();
            
            Assert.NotEqual<Guid>(Guid.Empty, streamId);
        }
        
        [Fact]
        public void ReturnDifferentIdEachTime_When_CallingNewForgettablePayloadId()
        {
            var streamId1 = ForgettablePayloadId.NewForgettablePayloadId();
            var streamId2 = ForgettablePayloadId.NewForgettablePayloadId();
            
            Assert.NotEqual(streamId1, streamId2);
        }
        
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithEmptyGuid()
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadId) Guid.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyGuid(Guid id)
        {
            ForgettablePayloadId _ = id;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(Guid value)
        {
            ForgettablePayloadId id1 = value;
            ForgettablePayloadId id2 = value;
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(Guid value, Guid otherValue)
        {
            ForgettablePayloadId id1 = value;
            ForgettablePayloadId id2 = otherValue;
            
            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(ForgettablePayloadId id)
        {
            var idAsGuid = (Guid) id;
            
            Assert.Equal(idAsGuid.ToString(), id.ToString());
        }
    }
}