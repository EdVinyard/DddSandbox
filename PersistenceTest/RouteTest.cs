using Domain;
using NUnit.Framework;

namespace PersistenceTest
{
    [TestFixture]
    public class RouteTest : DatabaseTest
    {
        [Test]
        public void Should_round_trip()
        {
            // Arrange
            var expected = new Route(
                "Road Trip",
                new Waypoint[0]);

            // Act
            DbSession.Save(expected);
            DbSession.Evict(expected);
            var actual = DbSession.Load<Route>(expected.RouteId);

            // Assert
            Assert.AreNotSame(expected, actual);
            Assert.NotNull(actual);

            Assert.AreEqual(expected.RouteId, actual.RouteId);
            Assert.AreEqual(expected.Label, actual.Label);
            Assert.AreEqual(expected.Waypoints, actual.Waypoints);
        }
    }
}
