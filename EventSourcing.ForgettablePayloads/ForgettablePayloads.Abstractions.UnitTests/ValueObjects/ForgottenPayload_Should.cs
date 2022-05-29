using System;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgottenPayload_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettingPayloadTime(
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgottenPayload(
                null,
                forgettingReason,
                forgettingRequestedBy));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettingPayloadReason(
            ForgettingPayloadTime forgettingTime,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgottenPayload(
                forgettingTime,
                null,
                forgettingRequestedBy));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettingPayloadRequestedBy(
            ForgettingPayloadTime forgettingTime,
            ForgettingPayloadReason forgettingReason)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgottenPayload(
                forgettingTime,
                forgettingReason,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            ForgettingPayloadTime forgettingTime,
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            _ = new ForgottenPayload(
                forgettingTime,
                forgettingReason,
                forgettingRequestedBy);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValuesProvidedDuringCreation_When_GettingPropertiesValues(
            ForgettingPayloadTime forgettingTime,
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            var forgottenPayload = new ForgottenPayload(
                forgettingTime,
                forgettingReason,
                forgettingRequestedBy);
            
            Assert.Equal(forgettingTime, forgottenPayload.ForgettingTime);
            Assert.Equal(forgettingReason, forgottenPayload.ForgettingReason);
            Assert.Equal(forgettingRequestedBy, forgottenPayload.ForgettingRequestedBy);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValuesProvidedDuringCreationAndCurrentForgettingPayloadTime_When_GettingPropertiesValues(
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            var forgottenPayload = ForgottenPayload.Create(
                forgettingReason,
                forgettingRequestedBy);
            
            Assert.True((DateTime.UtcNow - forgottenPayload.ForgettingTime).TotalMilliseconds <= 10);
            Assert.Equal(forgettingReason, forgottenPayload.ForgettingReason);
            Assert.Equal(forgettingRequestedBy, forgottenPayload.ForgettingRequestedBy);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            ForgettingPayloadTime forgettingTime,
            ForgettingPayloadReason forgettingReason,
            ForgettingPayloadRequestedBy forgettingRequestedBy)
        {
            var payload1 = new ForgottenPayload(
                forgettingTime,
                forgettingReason,
                forgettingRequestedBy);
            
            var payload2 = new ForgottenPayload(
                forgettingTime,
                forgettingReason,
                forgettingRequestedBy);
            
            Assert.Equal(payload1, payload2);
            Assert.True(payload1 == payload2);
            Assert.False(payload1 != payload2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentForgettingPayloadTime(
            ForgottenPayload payload,
            ForgettingPayloadTime differentForgettingPayloadTime)
        {
            var payload1 = new ForgottenPayload(
                payload.ForgettingTime,
                payload.ForgettingReason,
                payload.ForgettingRequestedBy);
            
            var payload2 = new ForgottenPayload(
                differentForgettingPayloadTime,
                payload.ForgettingReason,
                payload.ForgettingRequestedBy);
            
            Assert.NotEqual(payload1, payload2);
            Assert.False(payload1 == payload2);
            Assert.True(payload1 != payload2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentForgettingPayloadReason(
            ForgottenPayload payload,
            ForgettingPayloadReason differentForgettingPayloadReason)
        {
            var payload1 = new ForgottenPayload(
                payload.ForgettingTime,
                payload.ForgettingReason,
                payload.ForgettingRequestedBy);
            
            var payload2 = new ForgottenPayload(
                payload.ForgettingTime,
                differentForgettingPayloadReason,
                payload.ForgettingRequestedBy);
            
            Assert.NotEqual(payload1, payload2);
            Assert.False(payload1 == payload2);
            Assert.True(payload1 != payload2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentForgettingPayloadRequestedBy(
            ForgottenPayload payload,
            ForgettingPayloadRequestedBy differentForgettingPayloadRequestedBy)
        {
            var payload1 = new ForgottenPayload(
                payload.ForgettingTime,
                payload.ForgettingReason,
                payload.ForgettingRequestedBy);
            
            var payload2 = new ForgottenPayload(
                payload.ForgettingTime,
                payload.ForgettingReason,
                differentForgettingPayloadRequestedBy);
            
            Assert.NotEqual(payload1, payload2);
            Assert.False(payload1 == payload2);
            Assert.True(payload1 != payload2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(ForgottenPayload payload)
        {
            var expectedValue =
                $"Forgetting Time: {payload.ForgettingTime}, Forgetting Reason: {payload.ForgettingReason}, Forgetting Requested By: {payload.ForgettingRequestedBy}";
            
            Assert.Equal(expectedValue, payload.ToString());
        }
    }
}