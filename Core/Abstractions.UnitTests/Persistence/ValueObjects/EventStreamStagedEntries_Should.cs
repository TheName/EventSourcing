using System;
using EventSourcing.Persistence.ValueObjects;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.ValueObjects
{
    public class EventStreamStagedEntries_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullStagingId(
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntries(
                null,
                stagingTime,
                entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullStagingTime(
            EventStreamStagingId stagingId,
            EventStreamEntries entries)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntries(
                stagingId,
                null,
                entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEntries(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            _ = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStagingIdProvidedDuringCreation_When_GettingStagingId(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);

            Assert.Equal(stagingId, stagedEntries.StagingId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStagingTimeProvidedDuringCreation_When_GettingStagingTime(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);

            Assert.Equal(stagingTime, stagedEntries.StagingTime);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEntriesProvidedDuringCreation_When_GettingEntries(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);

            Assert.Equal(entries, stagedEntries.Entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamStagedEntries stagedEntries)
        {
            var stagedEntries1 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                stagedEntries.StagingTime,
                stagedEntries.Entries);

            var stagedEntries2 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                stagedEntries.StagingTime,
                stagedEntries.Entries);

            Assert.Equal(stagedEntries1, stagedEntries2);
            Assert.True(stagedEntries1 == stagedEntries2);
            Assert.False(stagedEntries1 != stagedEntries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStagingId(
            EventStreamStagedEntries stagedEntries,
            EventStreamStagingId differentStagingId)
        {
            var stagedEntries1 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                stagedEntries.StagingTime,
                stagedEntries.Entries);

            var stagedEntries2 = new EventStreamStagedEntries(
                differentStagingId,
                stagedEntries.StagingTime,
                stagedEntries.Entries);

            Assert.NotEqual(stagedEntries1, stagedEntries2);
            Assert.False(stagedEntries1 == stagedEntries2);
            Assert.True(stagedEntries1 != stagedEntries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStagingTime(
            EventStreamStagedEntries stagedEntries,
            EventStreamStagedEntriesStagingTime differentStagingTime)
        {
            var stagedEntries1 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                stagedEntries.StagingTime,
                stagedEntries.Entries);

            var stagedEntries2 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                differentStagingTime,
                stagedEntries.Entries);

            Assert.NotEqual(stagedEntries1, stagedEntries2);
            Assert.False(stagedEntries1 == stagedEntries2);
            Assert.True(stagedEntries1 != stagedEntries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEntries(
            EventStreamStagedEntries stagedEntries,
            EventStreamEntries differentEntries)
        {
            var stagedEntries1 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                stagedEntries.StagingTime,
                stagedEntries.Entries);

            var stagedEntries2 = new EventStreamStagedEntries(
                stagedEntries.StagingId,
                stagedEntries.StagingTime,
                differentEntries);

            Assert.NotEqual(stagedEntries1, stagedEntries2);
            Assert.False(stagedEntries1 == stagedEntries2);
            Assert.True(stagedEntries1 != stagedEntries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamStagedEntries stagedEntries)
        {
            var expectedValue =
                $"Event Stream Staging ID: {stagedEntries.StagingId}, Staging Time: {stagedEntries.StagingTime}, Entries: {stagedEntries.Entries}";

            Assert.Equal(expectedValue, stagedEntries.ToString());
        }
    }
}
