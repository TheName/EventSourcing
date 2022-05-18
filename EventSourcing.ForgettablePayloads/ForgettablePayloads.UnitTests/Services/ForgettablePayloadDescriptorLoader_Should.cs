using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;
using EventSourcing.ForgettablePayloads.Services;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Services
{
    public class ForgettablePayloadDescriptorLoader_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_Creating_With_NullStorageReader()
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptorLoader(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_With_NotNullParameters(
            IForgettablePayloadStorageReader storageReader)
        {
            _ = new ForgettablePayloadDescriptorLoader(storageReader);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Loading_And_PayloadIdIsNull(
            ForgettablePayloadDescriptorLoader loader)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => loader.LoadAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task ReturnDescriptorFromStorageReader_When_Loading(
            ForgettablePayloadId payloadId,
            ForgettablePayloadDescriptor readDescriptor,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            ForgettablePayloadDescriptorLoader loader)
        {
            storageReaderMock
                .Setup(reader => reader.ReadAsync(payloadId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(readDescriptor)
                .Verifiable();

            var result = await loader.LoadAsync(payloadId, CancellationToken.None);

            Assert.Equal(readDescriptor, result);
            storageReaderMock.Verify();
            storageReaderMock.VerifyNoOtherCalls();
        }
    }
}