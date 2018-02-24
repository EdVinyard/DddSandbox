using Domain.Aggregate.Auction;
using NUnit.Framework;
using System;

namespace DomainTest
{
    [TestFixture]
    public class TimeRangeTest
    {
        [Test]
        public void WhenDurationIsPositive()
        {
            // Arrange
            var expectedStart = DateTimeOffset.Parse("2017-01-15T23:50Z");
            var expectedEnd   = DateTimeOffset.Parse("2017-01-15T23:50:01Z");
            var duration = TimeSpan.FromSeconds(1);

            // Act
            var tr = new TimeRange(expectedStart, duration);

            // Assert
            Assert.AreEqual(duration, tr.Duration);
            Assert.AreEqual(expectedStart, tr.Start);
            Assert.AreEqual(expectedEnd, tr.End);
            Assert.IsTrue(tr.Includes(expectedStart));
            Assert.IsFalse(tr.Includes(expectedEnd));
        }

        [Test]
        public void WhenDurationIsNegative()
        {
            // Arrange
            var expectedStart = DateTimeOffset.Parse("2017-01-15T23:49:59Z");
            var expectedEnd   = DateTimeOffset.Parse("2017-01-15T23:50Z");
            var duration = TimeSpan.FromSeconds(-1);

            // Act
            var tr = new TimeRange(expectedEnd, duration);

            // Assert
            Assert.AreEqual(duration.Negate(), tr.Duration);
            Assert.AreEqual(expectedStart, tr.Start);
            Assert.AreEqual(expectedEnd, tr.End);
            Assert.IsTrue(tr.Includes(expectedStart));
            Assert.IsFalse(tr.Includes(expectedEnd));
        }

        [Test]
        public void WhenDurationIsZero()
        {
            // Arrange
            var anchor = DateTimeOffset.Parse("2017-01-15T23:50Z");

            // Act
            var tr = new TimeRange(anchor, TimeSpan.Zero);

            // Assert
            Assert.AreEqual(TimeSpan.Zero, tr.Duration);
            Assert.AreEqual(anchor, tr.Start);
            Assert.AreEqual(anchor, tr.End);
            Assert.IsTrue(tr.Includes(anchor));
        }

        [Test]
        public void WhenStartTimeIsEarlierThanDateTimeOffsetMinimum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new TimeRange(DateTimeOffset.MaxValue, TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void WhenEndTimeIsLaterThanDateTimeOffsetMaximum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TimeRange(DateTimeOffset.MinValue, TimeSpan.FromSeconds(-1)));
        }
    }
}
