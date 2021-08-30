﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;

namespace EventSourcing.Extensions
{
    internal static class EventSourcingBusPublisherExtensions
    {
        public static async Task PublishAsync(
            this IEventSourcingBusPublisher publisher,
            IEnumerable<EventStreamEntry> entries,
            CancellationToken cancellationToken)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            foreach (var eventStreamEntry in entries)
            {
                await publisher.PublishAsync(eventStreamEntry, cancellationToken).ConfigureAwait(false);
                
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}