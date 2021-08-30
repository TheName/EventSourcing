using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.Enums;
using TestHelpers.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class SqlServerEventStreamWriteRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        private IEventStreamWriteRepository Repository => _fixture.GetService<IEventStreamWriteRepository>();

        public SqlServerEventStreamWriteRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSuccess_When_WritingSingleEntry(EventStreamEntry entry)
        {
            var result = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            
            Assert.Equal(EventStreamWriteResult.Success, result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSequenceAlreadyTaken_When_WritingSingleEntryTwice(EventStreamEntry entry)
        {
            var firstResult = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, firstResult);
            
            var secondResult = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.SequenceAlreadyTaken, secondResult);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSuccess_When_WritingMultipleEntries(EventStreamEntries entries)
        {
            var result = await Repository.WriteAsync(entries, CancellationToken.None);
            
            Assert.Equal(EventStreamWriteResult.Success, result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSequenceAlreadyTaken_When_WritingMultipleEntriesTwice(EventStreamEntries entries)
        {
            var firstResult = await Repository.WriteAsync(entries, CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, firstResult);
            
            var secondResult = await Repository.WriteAsync(entries, CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.SequenceAlreadyTaken, secondResult);
        }
    }
}