using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.ForgettablePayloads.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Services
{
    public class ForgettablePayloadForgettingService_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullContentConverter(
            IForgettablePayloadStorageWriter storageWriter)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadForgettingService(
                null,
                storageWriter));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullStorageWriter(
            IForgettablePayloadContentConverter contentConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadForgettingService(
                contentConverter,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_With_NotNullParameters(
            IForgettablePayloadContentConverter contentConverter,
            IForgettablePayloadStorageWriter storageWriter)
        {
            _ = new ForgettablePayloadForgettingService(contentConverter, storageWriter);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Forgetting_And_DescriptorIsNull(
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            ForgettablePayloadForgettingService forgettingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => forgettingService.ForgetAsync(
                null,
                reason,
                requestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Forgetting_And_ReasonIsNull(
            ForgettablePayloadDescriptor descriptor,
            ForgettingPayloadRequestedBy requestedBy,
            ForgettablePayloadForgettingService forgettingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => forgettingService.ForgetAsync(
                descriptor,
                null,
                requestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Forgetting_And_RequestedByIsNull(
            ForgettablePayloadDescriptor descriptor,
            ForgettingPayloadReason reason,
            ForgettablePayloadForgettingService forgettingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => forgettingService.ForgetAsync(
                descriptor,
                reason,
                null,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_Forgetting_And_ContentConverterReturnsNull(
            ForgettablePayloadDescriptor descriptor,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            [Frozen] Mock<IForgettablePayloadContentConverter> contentConverterMock,
            ForgettablePayloadForgettingService forgettingService)
        {
            var expectedForgottenPayload = ForgottenPayload.Create(reason, requestedBy);
            var assertForgottenPayload = new Func<ForgottenPayload, bool>(payload =>
            {
                Assert.Equal(expectedForgottenPayload.ForgettingTime.Value, payload.ForgettingTime.Value, TimeSpan.FromMilliseconds(10));
                Assert.Equal(expectedForgottenPayload.ForgettingReason, payload.ForgettingReason);
                Assert.Equal(expectedForgottenPayload.ForgettingRequestedBy, payload.ForgettingRequestedBy);

                return true;
            });

            contentConverterMock
                .Setup(converter => converter.ToPayloadContentDescriptor(It.Is<ForgottenPayload>(payload => assertForgottenPayload(payload))))
                .Returns(null as ForgettablePayloadContentDescriptor)
                .Verifiable();
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettingService.ForgetAsync(
                descriptor,
                reason,
                requestedBy,
                CancellationToken.None));
            
            contentConverterMock.Verify();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ForgetAndUpdateForgettablePayloadDescriptor_When_Forgetting(
            ForgettablePayloadDescriptor descriptor,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            ForgettablePayloadContentDescriptor forgettablePayloadContentDescriptor,
            [Frozen] Mock<IForgettablePayloadStorageWriter> storageWriterMock,
            [Frozen] Mock<IForgettablePayloadContentConverter> contentConverterMock,
            ForgettablePayloadForgettingService forgettingService)
        {
            // Arrange
            var expectedForgottenPayload = ForgottenPayload.Create(reason, requestedBy);
            
            var assertForgottenPayload = new Func<ForgottenPayload, bool>(payload =>
            {
                Assert.Equal(expectedForgottenPayload.ForgettingTime.Value, payload.ForgettingTime.Value, TimeSpan.FromMilliseconds(100));
                Assert.Equal(expectedForgottenPayload.ForgettingReason, payload.ForgettingReason);
                Assert.Equal(expectedForgottenPayload.ForgettingRequestedBy, payload.ForgettingRequestedBy);

                return true;
            });

            contentConverterMock
                .Setup(converter => converter
                    .ToPayloadContentDescriptor(It.Is<ForgottenPayload>(payload => assertForgottenPayload(payload))))
                .Returns(forgettablePayloadContentDescriptor);

            // Act
            var result = await forgettingService.ForgetAsync(
                descriptor,
                reason,
                requestedBy,
                CancellationToken.None);

            // Assert
            var expectedForgottenDescriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                descriptor.ToMetadata().CreateUpdated(ForgettablePayloadState.Forgotten),
                forgettablePayloadContentDescriptor);

            var assertForgottenPayloadDescriptor = new Func<ForgettablePayloadDescriptor, bool>(payloadDescriptor =>
            {
                Assert.Equal(expectedForgottenDescriptor.EventStreamId, result.EventStreamId);
                Assert.Equal(expectedForgottenDescriptor.EventStreamEntryId, result.EventStreamEntryId);
                Assert.Equal(expectedForgottenDescriptor.PayloadId, result.PayloadId);
                Assert.Equal(expectedForgottenDescriptor.PayloadState, result.PayloadState);
                Assert.Equal(expectedForgottenDescriptor.PayloadCreationTime, result.PayloadCreationTime);
                Assert.Equal(expectedForgottenDescriptor.PayloadLastModifiedTime.Value, result.PayloadLastModifiedTime, TimeSpan.FromMilliseconds(100));
                Assert.Equal(expectedForgottenDescriptor.PayloadSequence, result.PayloadSequence);
                Assert.Equal(expectedForgottenDescriptor.PayloadContent, result.PayloadContent);
                Assert.Equal(expectedForgottenDescriptor.PayloadContentSerializationFormat, result.PayloadContentSerializationFormat);
                Assert.Equal(expectedForgottenDescriptor.PayloadTypeIdentifier, result.PayloadTypeIdentifier);
                Assert.Equal(expectedForgottenDescriptor.PayloadTypeIdentifierFormat, result.PayloadTypeIdentifierFormat);

                return true;
            });

            Assert.True(assertForgottenPayloadDescriptor(result));
            storageWriterMock.Verify(
                writer => writer.UpdateAsync(
                    It.Is<ForgettablePayloadDescriptor>(payloadDescriptor =>
                        assertForgottenPayloadDescriptor(payloadDescriptor)), It.IsAny<CancellationToken>()),
                Times.Once);
            
            storageWriterMock.VerifyNoOtherCalls();
            contentConverterMock.Verify();
        }
    }
}