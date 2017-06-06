using System;
using System.Device.Location;
using Domain;
using NUnit.Framework;

namespace PersistenceTest
{
    [TestFixture]
    public class WaypointTest : DatabaseTest
    {
        /// <summary>
        /// Pacific Daylight Time (PDT) UTC Offset
        /// </summary>
        private static readonly TimeSpan PDT = TimeSpan.FromHours(-8);

        private static readonly Location SanDiegoZoo = new Location(
            "2920 Zoo Dr, San Diego, CA 92101",
            new GeoCoordinate(32.736, -117.151));
        private static readonly TimeRange NineAmToSixPmPDT = new TimeRange(
            new System.DateTimeOffset(2017,06,06, 09,00,00, PDT), 
            TimeSpan.FromHours(8));

        [Test]
        public void Should_round_trip()
        {
            // Arrange
            var expected = new Waypoint(
                SanDiegoZoo,
                NineAmToSixPmPDT);

            // Act
            DbSession.Save(expected);
            DbSession.Evict(expected);
            var actual = DbSession.Load<Waypoint>(expected.WaypointId);

            // Assert
            Assert.AreNotSame(expected, actual);
            Assert.NotNull(actual);

            Assert.AreEqual(expected.Place, actual.Place);
            Assert.AreEqual(expected.Time, actual.Time);
        }
    }
}
