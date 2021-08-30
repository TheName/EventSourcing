using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.Enums;

namespace EventSourcing
{
    internal class EventStreamPublisher : IEventStreamPublisher
    {
        private readonly IEventStreamStagingWriter _stagingWriter;
        private readonly IEventStreamWriter _streamWriter;
        private readonly IEventSourcingBusPublisher _busPublisher;

        public EventStreamPublisher(
            IEventStreamStagingWriter stagingWriter,
            IEventStreamWriter streamWriter,
            IEventSourcingBusPublisher busPublisher)
        {
            _stagingWriter = stagingWriter ?? throw new ArgumentNullException(nameof(stagingWriter));
            _streamWriter = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
            _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
        }

        public async Task PublishAsync(EventStream stream, CancellationToken cancellationToken)
        {
            var eventsToAppend = stream?.EntriesToAppend ?? throw new ArgumentNullException(nameof(stream));
            if (eventsToAppend.Count == 0)
            {
                return;
            }

            var stagingId = await _stagingWriter.WriteAsync(eventsToAppend, cancellationToken).ConfigureAwait(false);
            var writeResult = await _streamWriter.WriteAsync(eventsToAppend, cancellationToken).ConfigureAwait(false);
            switch (writeResult)
            {
                case EventStreamWriteResult.Success:
                    await _busPublisher.PublishAsync(eventsToAppend, cancellationToken).ConfigureAwait(false);
                    await _stagingWriter.MarkAsPublishedAsync(stagingId, cancellationToken).ConfigureAwait(false);
                    break;
                    
                case EventStreamWriteResult.SequenceAlreadyTaken:
                    await _stagingWriter.MarkAsFailedToStoreAsync(stagingId, cancellationToken).ConfigureAwait(false);
                    throw EventStreamOptimisticConcurrencyException.New(eventsToAppend.Count, stream.StreamId);
                
                case EventStreamWriteResult.UnknownFailure:
                    throw EventStreamAppendingFailedException.New(eventsToAppend.Count, stream.StreamId);
                
                default:
                    throw new InvalidEnumArgumentException(nameof(writeResult), (int) writeResult, typeof(EventStreamWriteResult));
            }
        }
    }
}