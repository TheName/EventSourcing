using System.Threading;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using Moq;

namespace ForgettablePayloads.Abstractions.UnitTests.Extensions
{
    internal static class ForgettablePayloadDescriptorLoaderMockExtensions
    {
        public static void SetupLoadAsync(
            this Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            ForgettablePayloadId payloadId,
            ForgettablePayloadDescriptor payloadDescriptor)
        {
            descriptorLoaderMock
                .Setup(loader => loader.LoadAsync(payloadId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payloadDescriptor)
                .Verifiable();
        }
    }
}