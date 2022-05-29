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
    public class ForgettablePayloadClaimingService_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_Creating_With_NullStorageWriter()
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadClaimingService(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_With_NotNullParameters(
            IForgettablePayloadStorageWriter storageWriter)
        {
            _ = new ForgettablePayloadClaimingService(storageWriter);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Claiming_And_PayloadDescriptorIsNull(
            ForgettablePayloadClaimingService claimingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                claimingService.ClaimAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task UpdateAndReturnClaimedDescriptor_When_Claiming(
            ForgettablePayloadDescriptor descriptor,
            [Frozen] Mock<IForgettablePayloadStorageWriter> storageWriterMock,
            ForgettablePayloadClaimingService claimingService)
        {
            var result = await claimingService.ClaimAsync(descriptor, CancellationToken.None);

            var expectedClaimedDescriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                descriptor.ToMetadata().CreateUpdated(ForgettablePayloadState.CreatedAndClaimed),
                descriptor.ToContentDescriptor());

            Assert.Equal(expectedClaimedDescriptor.EventStreamId, result.EventStreamId);
            Assert.Equal(expectedClaimedDescriptor.EventStreamEntryId, result.EventStreamEntryId);
            Assert.Equal(expectedClaimedDescriptor.PayloadId, result.PayloadId);
            Assert.Equal(expectedClaimedDescriptor.PayloadState, result.PayloadState);
            Assert.Equal(expectedClaimedDescriptor.PayloadCreationTime, result.PayloadCreationTime);
            Assert.Equal(expectedClaimedDescriptor.PayloadLastModifiedTime.Value, result.PayloadLastModifiedTime.Value, TimeSpan.FromMilliseconds(100));
            Assert.Equal(expectedClaimedDescriptor.PayloadSequence, result.PayloadSequence);
            Assert.Equal(expectedClaimedDescriptor.PayloadContent, result.PayloadContent);
            Assert.Equal(expectedClaimedDescriptor.PayloadContentSerializationFormat, result.PayloadContentSerializationFormat);
            Assert.Equal(expectedClaimedDescriptor.PayloadTypeIdentifier, result.PayloadTypeIdentifier);
            Assert.Equal(expectedClaimedDescriptor.PayloadTypeIdentifierFormat, result.PayloadTypeIdentifierFormat);
            
            storageWriterMock
                .Verify(writer => writer.UpdateAsync(result, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}