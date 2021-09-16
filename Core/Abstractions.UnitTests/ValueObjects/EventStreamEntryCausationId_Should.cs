using System;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEntryCausationId_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithEmptyGuid()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEntryCausationId) Guid.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyGuid(Guid id)
        {
            EventStreamEntryCausationId _ = id;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnCausationId_When_GettingCurrent_After_SettingIt(EventStreamEntryCausationId causationId)
        {
            EventStreamEntryCausationId.Current = null;
            EventStreamEntryCausationId.Current = causationId;

            var current = EventStreamEntryCausationId.Current;

            Assert.Equal(causationId, current);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_When_TryingToSetCurrentCausationIdToDifferentValue_And_CurrentCausationIdIsAlreadySet(
            EventStreamEntryCausationId causationId,
            EventStreamEntryCausationId differentCausationId)
        {
            EventStreamEntryCausationId.Current = null;
            EventStreamEntryCausationId.Current = causationId;
            Assert.Throws<InvalidOperationException>(() => EventStreamEntryCausationId.Current = differentCausationId);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_TryingToSetCurrentCausationIdToSameValue_And_CurrentCausationIdIsAlreadySet(
            EventStreamEntryCausationId causationId)
        {
            EventStreamEntryCausationId.Current = null;
            EventStreamEntryCausationId.Current = causationId;
            EventStreamEntryCausationId.Current = causationId;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnCurrentCausationIdEqualToCurrentCorrelationId_When_GettingCurrentCausationId_And_CurrentCausationIdIsNull(
            EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;
            EventStreamEntryCausationId.Current = null;

            var result = EventStreamEntryCausationId.Current;

            Assert.Equal<Guid>(correlationId, result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCausationIdSetInCurrentThread_When_GettingCurrentCausationIdFromChildThread(EventStreamEntryCausationId causationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(() =>
            {
                Assert.Equal(causationId, EventStreamEntryCausationId.Current);
                return Task.CompletedTask;
            });

            await assertion();
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCausationIdSetInCurrentThread_When_GettingCurrentCausationIdFromChildThread_And_AwaitingWithoutSynchronizationContext(EventStreamEntryCausationId causationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(() =>
            {
                Assert.Equal(causationId, EventStreamEntryCausationId.Current);
                return Task.CompletedTask;
            });

            await assertion().ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCausationIdSetInCurrentThread_When_GettingCurrentCausationIdFromChildThreadAfterAwaiting(EventStreamEntryCausationId causationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                Assert.Equal(causationId, EventStreamEntryCausationId.Current);
            });

            await assertion();
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCausationIdSetInCurrentThread_When_GettingCurrentCausationIdFromChildThreadAfterAwaiting_And_AwaitingWithoutSynchronizationContext(EventStreamEntryCausationId causationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                Assert.Equal(causationId, EventStreamEntryCausationId.Current);
            });

            await assertion().ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCausationIdInChildThread_And_CurrentCausationIdWasAlreadySetInCurrentThread(
            EventStreamEntryCausationId causationId,
            EventStreamEntryCausationId differentCausationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(() =>
            {
                EventStreamEntryCausationId.Current = differentCausationId;
                return Task.CompletedTask;
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion());
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCausationIdInChildThread_And_CurrentCausationIdWasAlreadySetInCurrentThread_And_AwaitingWithoutSynchronizationContext(
            EventStreamEntryCausationId causationId,
            EventStreamEntryCausationId differentCausationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(() =>
            {
                EventStreamEntryCausationId.Current = differentCausationId;
                return Task.CompletedTask;
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion()).ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCausationIdInChildThreadAfterAwaiting_And_CurrentCausationIdWasAlreadySetInCurrentThread(
            EventStreamEntryCausationId causationId,
            EventStreamEntryCausationId differentCausationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                EventStreamEntryCausationId.Current = differentCausationId;
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion());
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCausationIdInChildThreadAfterAwaiting_And_CurrentCausationIdWasAlreadySetInCurrentThread_And_AwaitingWithoutSynchronizationContext(
            EventStreamEntryCausationId causationId,
            EventStreamEntryCausationId differentCausationId)
        {
            EventStreamEntryCausationId.Current = causationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                EventStreamEntryCausationId.Current = differentCausationId;
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion()).ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(Guid value)
        {
            EventStreamEntryCausationId id1 = value;
            EventStreamEntryCausationId id2 = value;
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(Guid value, Guid otherValue)
        {
            EventStreamEntryCausationId id1 = value;
            EventStreamEntryCausationId id2 = otherValue;
            
            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamEntryCausationId id)
        {
            var idAsGuid = (Guid) id;
            
            Assert.Equal(idAsGuid.ToString(), id.ToString());
        }
    }
}