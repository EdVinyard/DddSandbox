using System;
using System.Collections.Generic;

namespace Domain.Aggregate.Auction
{
    /// <summary>
    /// An immutable "window" of time, with a specific start and end time.
    /// </summary>
    public class TimeRange : IEquatable<TimeRange>
    {
        /// <summary>
        /// An unsatisfiable TimeRange, such that there are no instants in time
        /// that are included in the range.
        /// </summary>
        public static readonly TimeRange Never = new TimeRange(
            DateTimeOffset.MinValue, 
            TimeSpan.Zero);

        /// <summary>
        /// <para>
        /// Create a new TimeRange instance with the specified anchor and 
        /// duration.  The duration may be either positive or negative, but the
        /// range always *includes* the earliest time and *excludes* the latest
        /// time.
        /// </para>
        /// <para>
        /// Example 1: Given 
        /// <c>
        /// new TimeRange(
        ///     DateTimeOffset.Parse("2017-03-26T13:45-06:00"), 
        ///     TimeSpan.FromMinutes(30))
        /// </c>
        /// The range will start at (and include) 13:45.
        /// The range will end at (and exclude) 14:15.
        /// Every time between the two is included in the range.
        /// </para>
        /// <para>
        /// Example 2: Given 
        /// <c>
        /// new TimeRange(
        ///     DateTimeOffset.Parse("2017-03-26T13:45-06:00"), 
        ///     TimeSpan.FromMinutes(-30))
        /// </c>
        /// The range will start at (and include) 13:15.
        /// The range will end at (and exclude) 13:45.
        /// Every time between the two is included in the range.
        /// </para>
        /// </summary>
        public TimeRange(DateTimeOffset anchor, TimeSpan duration)
        {
            Duration = duration.Duration();

            if (duration.IsPositive())
            {
                Start = anchor;
                try
                {
                    End = anchor + duration;
                }
                catch (ArgumentOutOfRangeException exc)
                {
                    throw new ArgumentOutOfRangeException(
                        "The End of the specified TimeRange is after DateTimeOffset.MaxValue.",
                        exc);
                }
            }
            else
            {
                End = anchor;
                try
                {
                    Start = anchor + duration;
                }
                catch (ArgumentOutOfRangeException exc)
                {
                    throw new ArgumentOutOfRangeException(
                        "The Start of the specified TimeRange is before DateTimeOffset.MinValue.",
                        exc);
                }
            }
        }

        /// <summary>
        /// FOR NHibernate and Terms.Constructor ONLY!
        /// </summary> 
        protected TimeRange() { }

        /// <summary>
        /// The earliest time included in the range.  For a time range that 
        /// covers 1:00 PM until 2:00 PM, this is 1:00 PM.  Always earlier
        /// than or equal to <c>End</c>.
        /// </summary>
        public DateTimeOffset Start { get; private set; }

        /// <summary>
        /// The earliest time NOT included in the range.  For a time range that
        /// covers 1:00PM until 2:00PM, this is 2:00 PM.  Always later
        /// than or equal to <c>Start</c>.
        /// </summary>
        public DateTimeOffset End { get; private set; }

        /// <summary>
        /// The (positive) duration of the time range.  That is, 
        /// <c>End - Start</c>.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Returns true iff the specified date-time is included in the range.
        /// If the value is equal to or later than <c>Start</c>, but earlier 
        /// than <c>End</c>, it is included.
        /// </summary>
        /// <returns>
        /// true iff <c>(Start <= dateTime < End)</c>
        /// </returns>
        public bool Includes(DateTimeOffset dateTime)
        {
            if (TimeSpan.Zero == Duration)
            {
                // When the TimeRange is only a single moment in time, it 
                // should still include that one moment.
                return Start == dateTime;
            }
            
            return (Start <= dateTime) && (dateTime < End);
        }

        public override bool Equals(object otherObj)
        {
            if (null == otherObj) return false;
            if (ReferenceEquals(this, otherObj)) return true;
            var otherTimeRange = otherObj as TimeRange;
            if (null == otherTimeRange) return false;
            return this.Equals(otherTimeRange);
        }

        public bool Equals(TimeRange other)
        {
            return (Start == other.Start)
                && (Duration == other.Duration);
        }

        public static bool operator ==(TimeRange l, TimeRange r) => Object.Equals(l, r);
        public static bool operator !=(TimeRange l, TimeRange r) => !(l == r);

        public override int GetHashCode()
        {
            var hashCode = -1589446268;
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(Start);
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(Duration);
            return hashCode;
        }
    }
}
