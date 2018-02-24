using Domain.Aggregate.Auction;
using Domain.Port;
using NUnit.Framework;
using System;
using System.Device.Location;

namespace PersistenceTest
{
    [TestFixture]
    public class ReverseAuctionTest : DatabaseTest
    {
        private FakeClock Clock;
        private FakeGeocoder Geocoder;
        private Terms.Constructor TermsConstructor;
        private ReverseAuction.Constructor ReverseAuctionConstructor;
        private Location.Constructor LocationConstructor;

        public class FakeGeocoder : IGeocoder
        {
            public override GeoCoordinate GeoCode(string address)
            {
                return new GeoCoordinate()
                {
                    Latitude = 30.2672,
                    Longitude = 97.7431,
                };
            }
        }

        public class FakeClock : IClock
        {
            public DateTimeOffset Now => new DateTimeOffset(
                2018, 2, 3,
                4, 13, 59,
                TimeSpan.FromHours(1));
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO: Real dependency injection.
            Clock = new FakeClock();
            Geocoder = new FakeGeocoder();
            TermsConstructor = new Terms.Constructor(Clock);
            ReverseAuctionConstructor = new ReverseAuction.Constructor(Clock);
            LocationConstructor = new Location.Constructor(Geocoder);
        }

        [Test]
        public void Should_round_trip()
        {
            // Arrange
            var here = LocationConstructor.New("right here");
            var there = LocationConstructor.New("way over there");
            var withinOneHour   = new TimeRange(Clock.Now, TimeSpan.FromHours(1));
            var withinTwoHours  = new TimeRange(Clock.Now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(Clock.Now, TimeSpan.FromMinutes(5));
            var pickup = new Waypoint(here, withinOneHour);
            var dropoff = new Waypoint(there, withinTwoHours);
            var expected = ReverseAuctionConstructor.New(
                TermsConstructor.New(pickup, dropoff, "no extra terms"),
                biddingAllowed: nextFiveMinutes);

            // Act
            DbSession.Save(expected);
            DbSession.Evict(expected);
            var actual = DbSession.Get<ReverseAuction>(expected.Id);

            // Assert
            Assert.NotNull(actual);
            Assert.AreNotSame(expected, actual);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.BuyerTerms, actual.BuyerTerms);
            Assert.AreEqual(expected.BiddingAllowed, actual.BiddingAllowed);
        }
    }
}
