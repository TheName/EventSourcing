using System;
using System.Threading.Tasks;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEntryCorrelationId_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithEmptyGuid()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEntryCorrelationId) Guid.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyGuid(Guid id)
        {
            EventStreamEntryCorrelationId _ = id;
        }

        [Fact]
        public void ReturnNotEmptyCorrelationId_When_CallingNewEventStreamEntryCorrelationId()
        {
            var result = EventStreamEntryCorrelationId.NewEventStreamEntryCorrelationId();

            Assert.NotNull(result);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnCorrelationId_When_GettingCurrent_After_SettingIt(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = null;
            EventStreamEntryCorrelationId.Current = correlationId;

            var current = EventStreamEntryCorrelationId.Current;

            Assert.Equal(correlationId, current);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_When_TryingToSetCurrentCorrelationIdToDifferentValue_And_CurrentCorrelationIdIsAlreadySet(
            EventStreamEntryCorrelationId correlationId,
            EventStreamEntryCorrelationId differentCorrelationId)
        {
            EventStreamEntryCorrelationId.Current = null;
            EventStreamEntryCorrelationId.Current = correlationId;
            Assert.Throws<InvalidOperationException>(() => EventStreamEntryCorrelationId.Current = differentCorrelationId);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_TryingToSetCurrentCorrelationIdToSameValue_And_CurrentCorrelationIdIsAlreadySet(
            EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = null;
            EventStreamEntryCorrelationId.Current = correlationId;
            EventStreamEntryCorrelationId.Current = correlationId;
        }

        [Fact]
        public void ReturnNotEmptyCurrentCorrelationId_When_GettingCurrentCorrelationId_After_CurrentCorrelationIdWasSetToNull()
        {
            EventStreamEntryCorrelationId.Current = null;

            var result = EventStreamEntryCorrelationId.Current;

            Assert.NotNull(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCorrelationIdSetInCurrentThread_When_GettingCurrentCorrelationIdFromChildThread(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(() =>
            {
                Assert.Equal(correlationId, EventStreamEntryCorrelationId.Current);
                return Task.CompletedTask;
            });

            await assertion();
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCorrelationIdSetInCurrentThread_When_GettingCurrentCorrelationIdFromChildThread_And_AwaitingWithoutSynchronizationContext(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(() =>
            {
                Assert.Equal(correlationId, EventStreamEntryCorrelationId.Current);
                return Task.CompletedTask;
            });

            await assertion().ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCorrelationIdSetInCurrentThread_When_GettingCurrentCorrelationIdFromChildThreadAfterAwaiting(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                Assert.Equal(correlationId, EventStreamEntryCorrelationId.Current);
            });

            await assertion();
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnCurrentCorrelationIdSetInCurrentThread_When_GettingCurrentCorrelationIdFromChildThreadAfterAwaiting_And_AwaitingWithoutSynchronizationContext(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                Assert.Equal(correlationId, EventStreamEntryCorrelationId.Current);
            });

            await assertion().ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCorrelationIdInChildThread_And_CurrentCorrelationIdWasAlreadySetInCurrentThread(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(() =>
            {
                EventStreamEntryCorrelationId.Current = EventStreamEntryCorrelationId.NewEventStreamEntryCorrelationId();
                return Task.CompletedTask;
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion());
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCorrelationIdInChildThread_And_CurrentCorrelationIdWasAlreadySetInCurrentThread_And_AwaitingWithoutSynchronizationContext(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(() =>
            {
                EventStreamEntryCorrelationId.Current = EventStreamEntryCorrelationId.NewEventStreamEntryCorrelationId();
                return Task.CompletedTask;
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion()).ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCorrelationIdInChildThreadAfterAwaiting_And_CurrentCorrelationIdWasAlreadySetInCurrentThread(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                EventStreamEntryCorrelationId.Current = EventStreamEntryCorrelationId.NewEventStreamEntryCorrelationId();
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion());
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_TryingToSetCurrentCorrelationIdInChildThreadAfterAwaiting_And_CurrentCorrelationIdWasAlreadySetInCurrentThread_And_AwaitingWithoutSynchronizationContext(EventStreamEntryCorrelationId correlationId)
        {
            EventStreamEntryCorrelationId.Current = correlationId;

            var assertion = new Func<Task>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                EventStreamEntryCorrelationId.Current = EventStreamEntryCorrelationId.NewEventStreamEntryCorrelationId();
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => assertion()).ConfigureAwait(false);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(Guid value)
        {
            EventStreamEntryCorrelationId id1 = value;
            EventStreamEntryCorrelationId id2 = value;
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(Guid value, Guid otherValue)
        {
            EventStreamEntryCorrelationId id1 = value;
            EventStreamEntryCorrelationId id2 = otherValue;
            
            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamEntryCorrelationId id)
        {
            var idAsGuid = (Guid) id;
            
            Assert.Equal(idAsGuid.ToString(), id.ToString());
        }
    }
}
