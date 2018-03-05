using Domain;
using Domain.Aggregate.Auction;
using Domain.Port;
using NUnit.Framework;
using Persistence;
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

        public class FakeEventBus : IInterAggregateEventBus
        {
            public void Publish(InterAggregateEvent anEvent)
            {
                Console.WriteLine($"Published: {anEvent}");
            }

            public void Subscribe<T>(Action<T> subscriber) where T : InterAggregateEvent
            {
                Console.WriteLine($"Registered subscription for {typeof(T).Name}.");
            }
        }

        protected override void Configure(ConfigurationExpression c)
        {
            c.For<IClock>().Use<FakeClock>();
            c.For<IGeocoder>().Use<FakeGeocoder>();
            c.For<IInterAggregateEventBus>().Use<FakeEventBus>();
            c.For<IReverseAuctionRepository>().Use<ReverseAuctionRepository>();
        }

        [Test]
        public void Should_round_trip()
        {
            // Arrange
            var clock = Container.GetInstance<IClock>();
            var factory = Container.GetInstance<ReverseAuctionAggregate.Factory>();
            var repository = Container.GetInstance<IReverseAuctionRepository>();
            var withinOneHour   = new TimeRange(clock.Now, TimeSpan.FromHours(1));
            var withinTwoHours  = new TimeRange(clock.Now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            // Act
            var expected = factory.New(
                pickupAddress: "right here",
                pickupTime: withinOneHour,
                dropoffAddress: "over there",
                dropoffTime: withinTwoHours,
                otherTerms: "other terms",
                biddingAllowed: nextFiveMinutes);
            repository.Save(expected);
            Session.Clear();
            var actual = repository.Get(expected.Id);

            // Assert
            Assert.NotNull(actual);
            Assert.AreNotSame(expected, actual);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.BuyerTerms, actual.BuyerTerms);
            Assert.AreEqual(expected.BiddingAllowed, actual.BiddingAllowed);
        }
    }
}
