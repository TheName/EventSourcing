﻿using System;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamId_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithEmptyGuid()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamId) Guid.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyGuid(Guid id)
        {
            EventStreamId _ = id;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(Guid value)
        {
            EventStreamId id1 = value;
            EventStreamId id2 = value;
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(Guid value, Guid otherValue)
        {
            EventStreamId id1 = value;
            EventStreamId id2 = otherValue;
            
            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamId id)
        {
            var idAsGuid = (Guid) id;
            
            Assert.Equal(idAsGuid.ToString(), id.ToString());
        }
    }
}