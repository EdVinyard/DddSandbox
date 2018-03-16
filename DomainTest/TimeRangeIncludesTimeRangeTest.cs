using Domain.Aggregate.Common;
using NUnit.Framework;
using System;

namespace DomainTest
{
    [TestFixture]
    public class TimeRangeIncludesTimeRangeTest
    {
        [Test]
        public void WhenBothAreNever()
        {
            // This is getting a little philosophical...
            Assert.IsFalse(TimeRange.Never.Includes(TimeRange.Never));
        }

        public static TimeRange[] NeverTestCases =
        {
            new TimeRange(DateTimeOffset.MinValue,  TimeSpan.Zero),
            new TimeRange(DateTimeOffset.MinValue,  TimeSpan.FromHours(1)),
            new TimeRange(DateTimeOffset.Now,       TimeSpan.FromHours(1)),
            new TimeRange(DateTimeOffset.Now,       TimeSpan.Zero),
            new TimeRange(DateTimeOffset.Now,       TimeSpan.FromHours(-1)),
            new TimeRange(DateTimeOffset.MaxValue,  TimeSpan.FromHours(-1)),
            new TimeRange(DateTimeOffset.MaxValue,  TimeSpan.Zero),
        };

        [Test, TestCaseSource("NeverTestCases")]
        public void WhenThisIsNever(TimeRange other)
        {
            Assert.IsFalse(TimeRange.Never.Includes(other));
        }

        [Test, TestCaseSource("NeverTestCases")]
        public void WhenOtherIsNever(TimeRange timeRange)
        {
            Assert.IsFalse(timeRange.Includes(TimeRange.Never));
        }

        public static TimeRange Reference = new TimeRange(
            DateTimeOffset.Parse("2018-01-01 00:00:00"),
            TimeSpan.FromDays(1));

        public static TimeRange[] IncludeTestCases =
        {
            // same start time
            new TimeRange(Reference.Start,  TimeSpan.Zero),
            new TimeRange(Reference.Start,  TimeSpan.FromHours(1)),
            new TimeRange(Reference.Start,  TimeSpan.FromDays(1)),

            // starting in the middle of Reference
            new TimeRange(Reference.Start.AddHours(12), TimeSpan.Zero),
            new TimeRange(Reference.Start.AddHours(12), TimeSpan.FromHours(1)),
            new TimeRange(Reference.Start.AddHours(12), TimeSpan.FromHours(12)),

            // same end time
            //new TimeRange(Reference.End,  TimeSpan.Zero), // Notice this is NOT included!
            new TimeRange(Reference.End,    TimeSpan.FromHours(-1)),
            new TimeRange(Reference.End,    TimeSpan.FromDays(-1)),
        };

        [Test, TestCaseSource("IncludeTestCases")]
        public void ShouldInclude(TimeRange other)
        {
            Assert.IsTrue(Reference.Includes(other));
        }

        public static TimeRange[] ExcludeTestCases =
        {
            // entirely too early
            new TimeRange(Reference.Start.AddDays(-1),  TimeSpan.FromDays(0.5)),

            // Start is too early; End is OK
            new TimeRange(Reference.Start.AddDays(-1),  TimeSpan.FromDays(1)),
            new TimeRange(Reference.Start.AddDays(-1),  TimeSpan.FromDays(1.5)),
            new TimeRange(Reference.Start.AddDays(-1),  TimeSpan.FromDays(2)),

            // Start is too early; End is too late
            new TimeRange(Reference.Start.AddDays(-1),  TimeSpan.FromDays(3)),

            // End is too late; Start is OK
            new TimeRange(Reference.Start,              TimeSpan.FromDays(2)),
            new TimeRange(Reference.Start.AddDays(0.5), TimeSpan.FromDays(2)),

            // entirely too late
            new TimeRange(Reference.End,                TimeSpan.Zero),
            new TimeRange(Reference.End,                TimeSpan.FromDays(1)),
            new TimeRange(Reference.End.AddDays(1),     TimeSpan.FromDays(1)),
        };

        [Test, TestCaseSource("ExcludeTestCases")]
        public void ShouldNotInclude(TimeRange other)
        {
            Assert.IsFalse(Reference.Includes(other));
        }

        public static readonly TimeRange SingleInstantRange = new TimeRange(
            DateTimeOffset.Parse("2018-01-01 00:00:00"), 
            TimeSpan.Zero);

        [Test]
        public void SingleInstantRangeIncludesSelf()
        {
            Assert.IsTrue(SingleInstantRange.Includes(SingleInstantRange));
        }

        public static readonly TimeRange[] NotIncludedInSingleInstantRange =  
        {
            // same start
            new TimeRange(
                SingleInstantRange.Start,
                TimeSpan.FromMilliseconds(1)),

            // same end
            new TimeRange(
                SingleInstantRange.Start.AddMilliseconds(-1),
                TimeSpan.FromMilliseconds(1)),

            // enveloping
            new TimeRange(
                SingleInstantRange.Start.AddMilliseconds(-1),
                TimeSpan.FromMilliseconds(2)),
        };

        [Test, TestCaseSource("NotIncludedInSingleInstantRange")]
        public void SingleInstantDoesNotInclude(TimeRange other)
        {
            Assert.IsFalse(SingleInstantRange.Includes(other));
        }
    }
}
