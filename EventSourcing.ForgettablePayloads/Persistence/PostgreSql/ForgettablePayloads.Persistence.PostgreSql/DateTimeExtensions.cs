using System;

namespace EventSourcing.ForgettablePayloads.Persistence.PostgreSql
{
    internal static class DateTimeExtensions
    {
        public static long GetMillisecondsLeftover(this DateTime dateTime)
        {
            return dateTime.Ticks % TimeSpan.TicksPerMillisecond;
        }
        
        public static DateTime RoundToMilliseconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks / TimeSpan.TicksPerMillisecond * TimeSpan.TicksPerMillisecond, dateTime.Kind);
        }
        
        public static DateTime AddMillisecondsLeftover(this DateTime dateTime, long millisecondsLeftover)
        {
            return new DateTime(dateTime.Ticks + millisecondsLeftover, dateTime.Kind);
        }
    }
}