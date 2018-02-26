using Domain.Aggregate.Auction;
using Domain.Port;
using NUnit.Framework;
using StructureMap;
using System;
using System.Device.Location;

namespace PersistenceTest
{
    [TestFixture]
    public class ReverseAuctionTest : DatabaseTest
    {
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

        protected override void Configure(ConfigurationExpression c)
        {
            c.For<IClock>().Use<FakeClock>();
            c.For<IGeocoder>().Use<FakeGeocoder>();
        }

        Location.Factory LocationFactory => Container.GetInstance<Location.Factory>();
        IClock Clock => Container.GetInstance<IClock>();
        Terms.Factory TermsFactory => Container.GetInstance<Terms.Factory>();
        ReverseAuction.Factory ReverseAuctionFactory => 
            Container.GetInstance<ReverseAuction.Factory>();

        [Test]
        public void Should_round_trip()
        {
            // Arrange
            var locationFactory = Container.GetInstance<Location.Factory>();
            var here = LocationFactory.New("right here");
            var there = LocationFactory.New("way over there");
            var withinOneHour   = new TimeRange(Clock.Now, TimeSpan.FromHours(1));
            var withinTwoHours  = new TimeRange(Clock.Now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(Clock.Now, TimeSpan.FromMinutes(5));
            var pickup = new Waypoint(here, withinOneHour);
            var dropoff = new Waypoint(there, withinTwoHours);
            var expected = ReverseAuctionFactory.New(
                TermsFactory.New(pickup, dropoff, "no extra terms"),
                biddingAllowed: nextFiveMinutes);

            // Act
            Session.Save(expected);
            Session.Evict(expected);
            var actual = Session.Get<ReverseAuction>(expected.Id);

            // Assert
            Assert.NotNull(actual);
            Assert.AreNotSame(expected, actual);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.BuyerTerms, actual.BuyerTerms);
            Assert.AreEqual(expected.BiddingAllowed, actual.BiddingAllowed);
        }
    }
}
