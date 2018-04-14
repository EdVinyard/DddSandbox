using Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain.Aggregate.Common
{
    // RULE: Any value type that is used by more than one
    // aggregate should be "promoted" to some common area to make
    // it clear that it is, e.g., a generic TimeRange rather than
    // specifically a ReverseAuction.TimeRange.  In general Value
    // Types should be more generically constructed than Entities,
    // because they're lower-level concepts and more likely to be
    // broadly applicable or useful.  The degenerate example of this
    // are the bulit-in types: int, string, etc. that'll be used
    // everywhere, but also DateTimeOffset, TimeSpan, other such
    // general concepts.
    //
    // Value Types MAY be shared by Aggregates within a Bounded 
    // Context.  Value Types SHOULD NOT be shared between Bounded 
    // Contexts.  Even if they initially appear to have
    // the same meaning within those separate Contexts, it is
    // likely that their meaning will evolve or be discovered over
    // time to differ slightly.  For example, one Bounded Context
    // may need to tie a specitic date and time, with an associated
    // currency conversion rate, to a price.  Another Context may
    // need only the amount and monetary unit.  The imperative to 
    // D.R.Y. must never outweight the need for
    // correct, appropriate, focused Types, without "extra" features 
    // or functionality, only appropriate in some other Context, that
    // distracts the developer reading the code of a Bounded Context.

    /// <summary>
    /// An immutable "window" of time, with a specific start and end time.
    /// </summary>
    public class TimeRange : DDD.ValueType, IEquatable<TimeRange>
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
                // a single instant
                return Start == dateTime;
            }

            // an extended window
            return (Start <= dateTime) && (dateTime < End); 
        }

        /// <summary>
        /// Returns true iff the specified TimeRange is entirely included within
        /// this range.
        /// If the value is equal to or later than <c>Start</c>, but earlier 
        /// than <c>End</c>, it is included.
        /// </summary>
        /// <returns>
        /// true iff <c>(Start <= dateTime < End)</c>
        /// </returns>
        public bool Includes(TimeRange other)
        {
            if (object.ReferenceEquals(this, Never)) return false;
            if (object.ReferenceEquals(other, Never)) return false;

            return this.Includes(other.Start)
                && (other.End <= this.End);
        }

        //// parameters
        //var dateTime = Expression.Parameter(typeof(DateTime));
        //var timeRange = Expression.Parameter(typeof(TimeRange));

        //// derived
        //var duration = Expression.Field(timeRange, nameof(Duration));
        //var start = Expression.Field(timeRange, nameof(Start));
        //var end = Expression.Field(timeRange, nameof(End));
        //var zero = Expression.Constant(TimeSpan.Zero);

        //return Expression.Or(
        //    Expression.And(
        //        // When the TimeRange is only a single moment in time, it 
        //        // should still include that one moment.
        //        Expression.Equal(zero, duration),
        //        Expression.Equal(start, dateTime)),
        //    Expression.And(
        //        Expression.LessThanOrEqual(start, dateTime),
        //        Expression.LessThan(dateTime, end)));

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

        public override string ToString()
        {
            if (object.ReferenceEquals(this, Never)) return "<never>";
            if (Duration == TimeSpan.Zero) return $"[{Start}]";
            return $"[{Start}, {End})";
        }
    }
}
