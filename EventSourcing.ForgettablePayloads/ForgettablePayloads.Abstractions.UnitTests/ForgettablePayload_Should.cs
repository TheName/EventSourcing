using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using ForgettablePayloads.Abstractions.UnitTests.Extensions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests
{
    public class ForgettablePayload_Should
    {
        #region CreateNew - generic
        
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingNewGenericWithNullPayload()
        {
            Assert.Throws<ArgumentNullException>(() => ForgettablePayload.CreateNew<object>(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingNewGenericWithNonNullPayload(object payload)
        {
            _ = ForgettablePayload.CreateNew<object>(payload);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnNonDefaultPayloadId_When_GettingPayloadId_After_CreatedNewGeneric(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);
            
            Assert.NotNull(forgettablePayload.PayloadId);
            Assert.NotEqual(Guid.Empty, forgettablePayload.PayloadId.Value);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_CallingIsForgottenAsync_After_CreatedNewGeneric(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            var result = await forgettablePayload.IsForgottenAsync(CancellationToken.None);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_CallingIsClaimedAsync_After_CreatedNewGeneric(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            var result = await forgettablePayload.IsClaimedAsync(CancellationToken.None);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnPayload_When_CallingGetPayloadAsync_After_CreatedNewGeneric(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            var result = await forgettablePayload.GetPayloadAsync(CancellationToken.None);

            Assert.Equal(payload, result);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingTryCreateMetadataForEventStreamIdAndEntryIdWithNullEventStreamId_After_CreatedNewGeneric(
            object payload,
            EventStreamEntryId entryId)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            Assert.Throws<ArgumentNullException>(() => forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                null,
                entryId,
                out _));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingTryCreateMetadataForEventStreamIdAndEntryIdWithNullEventStreamEntryId_After_CreatedNewGeneric(
            object payload,
            EventStreamId eventStreamId)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            Assert.Throws<ArgumentNullException>(() => forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                eventStreamId,
                null,
                out _));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrueAndNewMetadata_When_CallingTryCreateMetadataForEventStreamIdAndEntryIdWithNonNullParameters_After_CreatedNewGeneric(
            object payload,
            EventStreamId eventStreamId,
            EventStreamEntryId entryId)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            var result = forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                eventStreamId,
                entryId,
                out var metadata);

            Assert.True(result);
            Assert.NotNull(metadata);
            Assert.Equal(eventStreamId, metadata.EventStreamId);
            Assert.Equal(entryId, metadata.EventStreamEntryId);
            Assert.NotNull(metadata.PayloadId);
            Assert.NotEqual(Guid.Empty, metadata.PayloadId.Value);
            Assert.Equal(ForgettablePayloadState.Created, metadata.PayloadState);
            Assert.NotNull(metadata.PayloadCreationTime);
            Assert.NotNull(metadata.PayloadLastModifiedTime);
            Assert.Equal(metadata.PayloadCreationTime.Value, metadata.PayloadLastModifiedTime.Value);
            Assert.NotNull(metadata.PayloadSequence);
            Assert.Equal((uint)0, metadata.PayloadSequence.Value);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_After_CreatedNewGeneric(
            object payload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_After_CreatedNewGeneric(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew<object>(payload);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.ClaimAsync(CancellationToken.None));
        }

        #endregion

        #region CreateNew
        
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingNewWithNullPayload()
        {
            Assert.Throws<ArgumentNullException>(() => ForgettablePayload.CreateNew(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingNewWithNonNullPayload(object payload)
        {
            _ = ForgettablePayload.CreateNew(payload);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnNonDefaultPayloadId_When_GettingPayloadId_After_CreatedNew(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);
            
            Assert.NotNull(forgettablePayload.PayloadId);
            Assert.NotEqual(Guid.Empty, forgettablePayload.PayloadId.Value);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_CallingIsForgottenAsync_After_CreatedNew(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            var result = await forgettablePayload.IsForgottenAsync(CancellationToken.None);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnFalse_When_CallingIsClaimedAsync_After_CreatedNew(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            var result = await forgettablePayload.IsClaimedAsync(CancellationToken.None);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnPayload_When_CallingGetPayloadAsync_After_CreatedNew(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            var result = await forgettablePayload.GetPayloadAsync(CancellationToken.None);

            Assert.Equal(payload, result);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingTryCreateMetadataForEventStreamIdAndEntryIdWithNullEventStreamId_After_CreatedNew(
            object payload,
            EventStreamEntryId entryId)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            Assert.Throws<ArgumentNullException>(() => forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                null,
                entryId,
                out _));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingTryCreateMetadataForEventStreamIdAndEntryIdWithNullEventStreamEntryId_After_CreatedNew(
            object payload,
            EventStreamId eventStreamId)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            Assert.Throws<ArgumentNullException>(() => forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                eventStreamId,
                null,
                out _));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrueAndNewMetadata_When_CallingTryCreateMetadataForEventStreamIdAndEntryIdWithNonNullParameters_After_CreatedNew(
            object payload,
            EventStreamId eventStreamId,
            EventStreamEntryId entryId)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            var result = forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                eventStreamId,
                entryId,
                out var metadata);

            Assert.True(result);
            Assert.NotNull(metadata);
            Assert.Equal(eventStreamId, metadata.EventStreamId);
            Assert.Equal(entryId, metadata.EventStreamEntryId);
            Assert.NotNull(metadata.PayloadId);
            Assert.NotEqual(Guid.Empty, metadata.PayloadId.Value);
            Assert.Equal(ForgettablePayloadState.Created, metadata.PayloadState);
            Assert.NotNull(metadata.PayloadCreationTime);
            Assert.NotNull(metadata.PayloadLastModifiedTime);
            Assert.Equal(metadata.PayloadCreationTime.Value, metadata.PayloadLastModifiedTime.Value);
            Assert.NotNull(metadata.PayloadSequence);
            Assert.Equal((uint)0, metadata.PayloadSequence.Value);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_After_CreatedNew(
            object payload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_After_CreatedNew(object payload)
        {
            var forgettablePayload = ForgettablePayload.CreateNew(payload);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.ClaimAsync(CancellationToken.None));
        }

        #endregion

        #region Create
        
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullPayloadId()
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayload(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullPayloadId(ForgettablePayloadId payloadId)
        {
            _ = new ForgettablePayload(payloadId);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingIsForgottenAsync_And_DescriptorLoaderIsNotAssigned(
            ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.IsForgottenAsync(CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingIsForgottenAsync_And_LoadedDescriptorIsNull(
            ForgettablePayload forgettablePayload,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.IsForgottenAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingIsForgottenAsync_And_LoadedDescriptorHasDifferentPayloadIdThanForgettablePayload(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.IsForgottenAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), true)]
        public async Task LoadDescriptor_And_ReturnExpectedResult_When_CallingIsForgottenAsync(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            var result = await forgettablePayload.IsForgottenAsync(CancellationToken.None);
            
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), true)]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingIsForgottenAsync_After_IsForgottenAsyncWasAlreadyCalled(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.IsForgottenAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.IsForgottenAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), true)]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingIsForgottenAsync_After_IsClaimedAsyncWasAlreadyCalled(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.IsClaimedAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.IsForgottenAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), true)]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingIsForgottenAsync_After_GetPayloadAsyncWasAlreadyCalled(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            IForgettablePayloadContentConverter contentConverter)
        {
            // Arrange
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            
            // Needed to load payload
            forgettablePayload.AssignContentConverter(contentConverter);
            
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.GetPayloadAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.IsForgottenAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingIsClaimedAsync_And_DescriptorLoaderIsNotAssigned(
            ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.IsClaimedAsync(CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingIsClaimedAsync_And_LoadedDescriptorIsNull(
            ForgettablePayload forgettablePayload,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.IsClaimedAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingIsClaimedAsync_And_LoadedDescriptorHasDifferentPayloadIdThanForgettablePayload(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.IsClaimedAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), true)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), false)]
        public async Task LoadDescriptor_And_ConvertPayload_And_ReturnExpectedResult_When_CallingIsClaimedAsync(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            var result = await forgettablePayload.IsClaimedAsync(CancellationToken.None);
            
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.Verify();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), true)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), false)]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingIsClaimedAsync_After_IsForgottenAsyncWasAlreadyCalled(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.IsForgottenAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.IsClaimedAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), true)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), false)]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingIsClaimedAsync_After_IsClaimedAsyncWasAlreadyCalled(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.IsClaimedAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.IsClaimedAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), false)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), true)]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), false)]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingIsClaimedAsync_After_GetPayloadAsyncWasAlreadyCalled(
            string payloadState,
            bool expectedResult,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            IForgettablePayloadContentConverter contentConverter)
        {
            // Arrange
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            
            // Needed to load payload
            forgettablePayload.AssignContentConverter(contentConverter);
            
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.GetPayloadAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.IsClaimedAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(expectedResult, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingGetPayloadAsync_And_DescriptorLoaderIsNotAssigned(
            ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.GetPayloadAsync(CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingGetPayloadAsync_And_LoadedDescriptorIsNull(
            ForgettablePayload forgettablePayload,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.GetPayloadAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingGetPayloadAsync_And_LoadedDescriptorHasDifferentPayloadIdThanForgettablePayload(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.GetPayloadAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingGetPayloadAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadConverterIsNotAssigned(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.GetPayloadAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingGetPayloadAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadConverterReturnsNull(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            payloadDescriptor = CreateWithId(payloadDescriptor, forgettablePayload.PayloadId);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.GetPayloadAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_CallingGetPayloadAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadConverterIsAssigned(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            IForgettablePayloadContentConverter contentConverter)
        {
            payloadDescriptor = CreateWithId(payloadDescriptor, forgettablePayload.PayloadId);
            forgettablePayload.AssignContentConverter(contentConverter);
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            _ = await forgettablePayload.GetPayloadAsync(CancellationToken.None);
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task LoadDescriptor_And_ConvertPayload_And_ReturnExpectedResult_When_CallingGetPayloadAsync(
            object payloadObject,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            payloadDescriptor = CreateWithId(payloadDescriptor, forgettablePayload.PayloadId);
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            forgettablePayload.AssignContentConverter(contentConverterMock.Object);
            contentConverterMock
                .Setup(converter => converter.FromPayloadContentDescriptor(payloadDescriptor.ToContentDescriptor()))
                .Returns(payloadObject)
                .Verifiable();

            var result = await forgettablePayload.GetPayloadAsync(CancellationToken.None);
            
            Assert.Equal(payloadObject, result);
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.Verify();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingGetPayloadAsync_After_IsForgottenAsyncWasAlreadyCalled(
            object payloadObject,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithId(payloadDescriptor, forgettablePayload.PayloadId);
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.IsForgottenAsync(CancellationToken.None));
            
            forgettablePayload.AssignContentConverter(contentConverterMock.Object);
            contentConverterMock
                .Setup(converter => converter.FromPayloadContentDescriptor(payloadDescriptor.ToContentDescriptor()))
                .Returns(payloadObject)
                .Verifiable();
            
            // Act
            var result = await forgettablePayload.GetPayloadAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(payloadObject, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.Verify();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingGetPayloadAsync_After_IsClaimedAsyncWasAlreadyCalled(
            object payloadObject,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithId(payloadDescriptor, forgettablePayload.PayloadId);
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock, 
                payload => payload.IsClaimedAsync(CancellationToken.None));
            
            forgettablePayload.AssignContentConverter(contentConverterMock.Object);
            contentConverterMock
                .Setup(converter => converter.FromPayloadContentDescriptor(payloadDescriptor.ToContentDescriptor()))
                .Returns(payloadObject)
                .Verifiable();
            
            // Act
            var result = await forgettablePayload.GetPayloadAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(payloadObject, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.Verify();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task DoNotReloadDescriptor_And_ReturnExpectedResult_When_CallingGetPayloadAsync_After_GetPayloadAsyncWasAlreadyCalled(
            object payloadObject,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadContentConverter> contentConverterMock)
        {
            // Arrange
            payloadDescriptor = CreateWithId(payloadDescriptor, forgettablePayload.PayloadId);
            forgettablePayload.AssignContentConverter(contentConverterMock.Object);
            contentConverterMock
                .Setup(converter => converter.FromPayloadContentDescriptor(payloadDescriptor.ToContentDescriptor()))
                .Returns(payloadObject)
                .Verifiable();
            
            await SetupLoading_And_ExecuteAction_And_ResetMocks(
                forgettablePayload,
                payloadDescriptor,
                descriptorLoaderMock,
                payload => payload.GetPayloadAsync(CancellationToken.None));
            
            // Act
            var result = await forgettablePayload.GetPayloadAsync(CancellationToken.None);
            
            // Assert
            Assert.Equal(payloadObject, result);
            descriptorLoaderMock.VerifyNoOtherCalls();
            contentConverterMock.Verify();
            contentConverterMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_CallingTryCreateMetadataForEventStreamIdAndEntryId(
            ForgettablePayload forgettablePayload,
            EventStreamId eventStreamId,
            EventStreamEntryId entryId)
        {
            var result = forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                eventStreamId,
                entryId,
                out _);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingForgetAsync_And_ForgettingPayloadReasonIsNull(
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.ForgetAsync(
                    null,
                    forgettingPayloadRequestedBy,
                    CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingForgetAsync_And_ForgettingPayloadRequestedByIsNull(
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.ForgetAsync(
                    forgettingPayloadReason,
                    null,
                    CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_And_DescriptorLoaderIsNotAssigned(
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.ForgetAsync(
                    forgettingPayloadReason,
                    forgettingPayloadRequestedBy,
                    CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingForgetAsync_And_LoadedDescriptorIsNull(
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.ForgetAsync(
                    forgettingPayloadReason,
                    forgettingPayloadRequestedBy,
                    CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_And_LoadedDescriptorHasDifferentPayloadIdThanForgettablePayload(
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.ForgetAsync(
                    forgettingPayloadReason,
                    forgettingPayloadRequestedBy,
                    CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten))]
        public async Task DoNothing_When_CallingForgetAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsAlreadyForgotten(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None);
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotForgotten_And_ForgettingServiceIsNotAssigned(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public async Task Throw_ArgumentNullException_When_CallingForgetAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotForgotten_And_ForgettingServiceIsAssigned_And_ForgettingServiceReturnsNull(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadForgettingService> forgettingServiceMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignForgettingService(forgettingServiceMock.Object);
            forgettingServiceMock.SetupForgettingResult(
                payloadDescriptor,
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotForgotten_And_ForgettingServiceIsAssigned_And_ForgettingServiceReturnsDescriptorWithDifferentPayloadId(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettablePayloadDescriptor payloadDescriptorReturnedFromForgettingService,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadForgettingService> forgettingServiceMock)
        {
            Assert.NotEqual(payloadDescriptor.PayloadId, payloadDescriptorReturnedFromForgettingService.PayloadId);
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignForgettingService(forgettingServiceMock.Object);
            forgettingServiceMock.SetupForgettingResult(
                payloadDescriptor,
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                payloadDescriptorReturnedFromForgettingService);

            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public async Task Throw_InvalidOperationException_When_CallingForgetAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotForgotten_And_ForgettingServiceIsAssigned_And_ForgettingServiceReturnsDescriptorWithSamePayloadIdAndStateNotForgotten(
            string payloadState,
            string payloadStateReturnedFromForgettingService,
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettablePayloadDescriptor payloadDescriptorReturnedFromForgettingService,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadForgettingService> forgettingServiceMock)
        {
            Assert.NotEqual(payloadDescriptor.PayloadId, payloadDescriptorReturnedFromForgettingService.PayloadId);
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            payloadDescriptorReturnedFromForgettingService = CreateWithIdAndState(payloadDescriptorReturnedFromForgettingService, payloadDescriptor.PayloadId, payloadStateReturnedFromForgettingService);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignForgettingService(forgettingServiceMock.Object);
            forgettingServiceMock.SetupForgettingResult(
                payloadDescriptor,
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                payloadDescriptorReturnedFromForgettingService);

            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None));
        }

        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.Forgotten))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), nameof(ForgettablePayloadState.Forgotten))]
        public async Task NotThrow_When_CallingForgetAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotForgotten_And_ForgettingServiceIsAssigned_And_ForgettingServiceReturnsDescriptorWithSamePayloadIdAndStateForgotten(
            string payloadState,
            string payloadStateReturnedFromForgettingService,
            ForgettablePayload forgettablePayload,
            ForgettingPayloadReason forgettingPayloadReason,
            ForgettingPayloadRequestedBy forgettingPayloadRequestedBy,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettablePayloadDescriptor payloadDescriptorReturnedFromForgettingService,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadForgettingService> forgettingServiceMock)
        {
            Assert.NotEqual(payloadDescriptor.PayloadId, payloadDescriptorReturnedFromForgettingService.PayloadId);
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            payloadDescriptorReturnedFromForgettingService = CreateWithIdAndState(payloadDescriptorReturnedFromForgettingService, payloadDescriptor.PayloadId, payloadStateReturnedFromForgettingService);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignForgettingService(forgettingServiceMock.Object);
            forgettingServiceMock.SetupForgettingResult(
                payloadDescriptor,
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                payloadDescriptorReturnedFromForgettingService);

            await forgettablePayload.ForgetAsync(
                forgettingPayloadReason,
                forgettingPayloadRequestedBy,
                CancellationToken.None);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_And_DescriptorLoaderIsNotAssigned(ForgettablePayload forgettablePayload)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettablePayload.ClaimAsync(CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_ArgumentNullException_When_CallingClaimAsync_And_LoadedDescriptorIsNull(
            ForgettablePayload forgettablePayload,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettablePayload.ClaimAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_And_LoadedDescriptorHasDifferentPayloadIdThanForgettablePayload(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
        
            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ClaimAsync(CancellationToken.None));
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public async Task DoNothing_When_CallingClaimAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsAlreadyClaimed(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
        
            await forgettablePayload.ClaimAsync(CancellationToken.None);
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten))]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotClaimed_And_ClaimingServiceIsNotAssigned(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
        
            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ClaimAsync(CancellationToken.None));
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten))]
        public async Task Throw_ArgumentNullException_When_CallingClaimAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotClaimed_And_ClaimingServiceIsAssigned_And_ClaimingServiceReturnsNull(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadClaimingService> claimingServiceMock)
        {
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignClaimingService(claimingServiceMock.Object);
            claimingServiceMock
                .Setup(service => service.ClaimAsync(payloadDescriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ForgettablePayloadDescriptor);
        
            await Assert.ThrowsAsync<ArgumentNullException>(() => forgettablePayload.ClaimAsync(CancellationToken.None));
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten))]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotClaimed_And_ClaimingServiceIsAssigned_And_ClaimingServiceReturnsDescriptorWithDifferentPayloadId(
            string payloadState,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettablePayloadDescriptor payloadDescriptorReturnedFromClaimingService,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadClaimingService> claimingServiceMock)
        {
            Assert.NotEqual(payloadDescriptor.PayloadId, payloadDescriptorReturnedFromClaimingService.PayloadId);
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignClaimingService(claimingServiceMock.Object);
            claimingServiceMock
                .Setup(service => service.ClaimAsync(payloadDescriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payloadDescriptorReturnedFromClaimingService); 
        
            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ClaimAsync(CancellationToken.None));
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.Forgotten))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), nameof(ForgettablePayloadState.Created))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), nameof(ForgettablePayloadState.Forgotten))]
        public async Task Throw_InvalidOperationException_When_CallingClaimAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotClaimed_And_ClaimingServiceIsAssigned_And_ClaimingServiceReturnsDescriptorWithSamePayloadIdAndStateNotClaimed(
            string payloadState,
            string payloadStateReturnedFromClaimingService,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettablePayloadDescriptor payloadDescriptorReturnedFromClaimingService,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadClaimingService> claimingServiceMock)
        {
            Assert.NotEqual(payloadDescriptor.PayloadId, payloadDescriptorReturnedFromClaimingService.PayloadId);
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            payloadDescriptorReturnedFromClaimingService = CreateWithIdAndState(payloadDescriptorReturnedFromClaimingService, payloadDescriptor.PayloadId, payloadStateReturnedFromClaimingService);

            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignClaimingService(claimingServiceMock.Object);
            claimingServiceMock
                .Setup(service => service.ClaimAsync(payloadDescriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payloadDescriptorReturnedFromClaimingService);
        
            await Assert.ThrowsAsync<InvalidOperationException>(() => forgettablePayload.ClaimAsync(CancellationToken.None));
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [AutoMoqWithInlineData(nameof(ForgettablePayloadState.Forgotten), nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public async Task NotThrow_When_CallingClaimAsync_And_LoadedDescriptorHasSamePayloadIdAsForgettablePayload_And_PayloadIsNotClaimed_And_ClaimingServiceIsAssigned_And_ClaimingServiceReturnsDescriptorWithSamePayloadIdAndStateClaimed(
            string payloadState,
            string payloadStateReturnedFromClaimingService,
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            ForgettablePayloadDescriptor payloadDescriptorReturnedFromClaimingService,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Mock<IForgettablePayloadClaimingService> claimingServiceMock)
        {
            Assert.NotEqual(payloadDescriptor.PayloadId, payloadDescriptorReturnedFromClaimingService.PayloadId);
            payloadDescriptor = CreateWithIdAndState(payloadDescriptor, forgettablePayload.PayloadId, payloadState);
            payloadDescriptorReturnedFromClaimingService = CreateWithIdAndState(payloadDescriptorReturnedFromClaimingService, payloadDescriptor.PayloadId, payloadStateReturnedFromClaimingService);
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);
            
            forgettablePayload.AssignClaimingService(claimingServiceMock.Object);
            claimingServiceMock
                .Setup(service => service.ClaimAsync(payloadDescriptor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payloadDescriptorReturnedFromClaimingService);
        
            await forgettablePayload.ClaimAsync(CancellationToken.None);
        }

        #endregion

        #region Assignments

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AssigningPayloadDescriptorLoaderService_And_ServiceIsNull(ForgettablePayload forgettablePayload)
        {
            Assert.Throws<ArgumentNullException>(() => forgettablePayload.AssignPayloadDescriptorLoaderService(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AssigningPayloadDescriptorLoaderService_And_ServiceIsNotNull(
            ForgettablePayload forgettablePayload,
            IForgettablePayloadDescriptorLoader descriptorLoader)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoader);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AssigningForgettingService_And_ServiceIsNull(ForgettablePayload forgettablePayload)
        {
            Assert.Throws<ArgumentNullException>(() => forgettablePayload.AssignForgettingService(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AssigningForgettingService_And_ServiceIsNotNull(
            ForgettablePayload forgettablePayload,
            IForgettablePayloadForgettingService forgettingService)
        {
            forgettablePayload.AssignForgettingService(forgettingService);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AssigningClaimingService_And_ServiceIsNull(ForgettablePayload forgettablePayload)
        {
            Assert.Throws<ArgumentNullException>(() => forgettablePayload.AssignClaimingService(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AssigningClaimingService_And_ServiceIsNotNull(
            ForgettablePayload forgettablePayload,
            IForgettablePayloadClaimingService claimingService)
        {
            forgettablePayload.AssignClaimingService(claimingService);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AssigningContentConverter_And_ConverterIsNull(ForgettablePayload forgettablePayload)
        {
            Assert.Throws<ArgumentNullException>(() => forgettablePayload.AssignContentConverter(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AssigningContentConverter_And_ConverterIsNotNull(
            ForgettablePayload forgettablePayload,
            IForgettablePayloadContentConverter contentConverter)
        {
            forgettablePayload.AssignContentConverter(contentConverter);
        }

        #endregion

        private static ForgettablePayloadDescriptor CreateWithId(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadId id)
        {
            return new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                id,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
        }

        private static ForgettablePayloadDescriptor CreateWithIdAndState(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadId id,
            ForgettablePayloadState state)
        {
            return new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                id,
                state,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
        }

        private static async Task SetupLoading_And_ExecuteAction_And_ResetMocks(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadDescriptor payloadDescriptor,
            Mock<IForgettablePayloadDescriptorLoader> descriptorLoaderMock,
            Func<ForgettablePayload, Task> action)
        {
            forgettablePayload.AssignPayloadDescriptorLoaderService(descriptorLoaderMock.Object);
            descriptorLoaderMock.SetupLoadAsync(forgettablePayload.PayloadId, payloadDescriptor);

            await action(forgettablePayload);
            
            descriptorLoaderMock.Verify();
            descriptorLoaderMock.VerifyNoOtherCalls();
            
            descriptorLoaderMock.Reset();
        }
    }
}