using System.Threading;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.ForgettablePayloads.ValueObjects;
using Moq;

namespace ForgettablePayloads.Abstractions.UnitTests.Extensions
{
    internal static class ForgettablePayloadForgettingServiceMockExtensions
    {
        public static void SetupForgettingResult(
            this Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            ForgettablePayloadDescriptor returnedPayloadDescriptor)
        {
            forgettingServiceMock
                .Setup(forgettingService => forgettingService.ForgetAsync(
                    payloadDescriptor,
                    reason,
                    requestedBy,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnedPayloadDescriptor)
                .Verifiable();
        }
    }
}