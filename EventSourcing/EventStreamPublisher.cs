using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.EventBus.Abstractions;
using EventSourcing.Persistence.Abstractions;

namespace EventSourcing
{
    /// <inheritdoc />
    public class EventStreamPublisher : IEventStreamPublisher
    {
        private readonly IEventStreamStagingWriter _storeStagingWriter;
        private readonly IEventStreamWriter _storeWriter;
        private readonly IEventStreamBusPublisher _streamBusPublisher;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEntry"/> class.
        /// </summary>
        /// <param name="storeStagingWriter">
        /// The <see cref="IEventStreamStagingWriter"/>.
        /// </param>
        /// <param name="storeWriter">
        /// The <see cref="IEventStreamWriter"/>.
        /// </param>
        /// <param name="streamBusPublisher">
        /// The <see cref="IEventStreamBusPublisher"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of provided parameters is null.
        /// </exception>
        public EventStreamPublisher(
            IEventStreamStagingWriter storeStagingWriter,
            IEventStreamWriter storeWriter,
            IEventStreamBusPublisher streamBusPublisher)
        {
            _storeStagingWriter = storeStagingWriter ?? throw new ArgumentNullException(nameof(storeStagingWriter));
            _storeWriter = storeWriter ?? throw new ArgumentNullException(nameof(storeWriter));
            _streamBusPublisher = streamBusPublisher ?? throw new ArgumentNullException(nameof(streamBusPublisher));
        }

        /// <inheritdoc />
        public async Task PublishAsync(EventStream stream, CancellationToken cancellationToken)
        {
            var eventsToAppend = stream?.EntriesToAppend ?? throw new ArgumentNullException(nameof(stream));
            if (eventsToAppend.Count == 0)
            {
                return;
            }

            var stagingId = await _storeStagingWriter.WriteAsync(eventsToAppend, cancellationToken).ConfigureAwait(false);
            var writeResult = await _storeWriter.WriteAsync(eventsToAppend, cancellationToken).ConfigureAwait(false);
            switch (writeResult)
            {
                case EventStreamWriteResult.Success:
                    await _streamBusPublisher.PublishAsync(eventsToAppend, cancellationToken).ConfigureAwait(false);
                    await _storeStagingWriter.MarkAsPublishedAsync(stagingId, cancellationToken).ConfigureAwait(false);
                    break;
                    
                case EventStreamWriteResult.SequenceAlreadyTaken:
                    await _storeStagingWriter.MarkAsFailedToStoreAsync(stagingId, cancellationToken).ConfigureAwait(false);
                    throw EventStreamOptimisticConcurrencyException.New(eventsToAppend.Count, stream.StreamId);
                
                case EventStreamWriteResult.UnknownFailure:
                    await _storeStagingWriter.MarkAsFailedToStoreAsync(stagingId, cancellationToken).ConfigureAwait(false);
                    throw EventStreamAppendingFailedException.New(eventsToAppend.Count, stream.StreamId);
                
                default:
                    throw new InvalidEnumArgumentException(nameof(writeResult), (int) writeResult, typeof(EventStreamWriteResult));
            }
        }
    }
}