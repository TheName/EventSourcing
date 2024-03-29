﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Aggregates.Conversion;

namespace EventSourcing.Aggregates.Publishers
{
    internal class EventStreamAggregatePublisher : IEventStreamAggregatePublisher
    {
        private readonly IEventStreamAggregateConverter _aggregateConverter;
        private readonly IEventStreamPublisher _eventStreamPublisher;

        public EventStreamAggregatePublisher(
            IEventStreamAggregateConverter aggregateConverter,
            IEventStreamPublisher eventStreamPublisher)
        {
            _aggregateConverter = aggregateConverter ?? throw new ArgumentNullException(nameof(aggregateConverter));
            _eventStreamPublisher = eventStreamPublisher ?? throw new ArgumentNullException(nameof(eventStreamPublisher));
        }

        public async Task PublishAsync(object aggregate, CancellationToken cancellationToken)
        {
            var publishableStream = _aggregateConverter.ToPublishableEventStream(aggregate);

            await _eventStreamPublisher.PublishAsync(publishableStream, cancellationToken).ConfigureAwait(false);
        }
    }
}
