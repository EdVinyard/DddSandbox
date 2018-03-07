using Domain;
using Domain.Aggregate.Auction;
using Domain.Port;
using NHibernate;
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
            private static readonly Random _prng = new Random();
            public override GeoCoordinate GeoCode(string address)
            {
                return new GeoCoordinate()
                {
                    Latitude = 30 + _prng.NextDouble(),
                    Longitude = 97 + _prng.NextDouble(),
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
        public void ShouldRoundTrip()
        {
            // Arrange
            var expected = NewReverseAuction();
            var repository = Container.GetInstance<IReverseAuctionRepository>();

            // Act
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

        private ReverseAuctionAggregate NewReverseAuction()
        {
            var clock = Container.GetInstance<IClock>();
            var factory = Container.GetInstance<ReverseAuctionAggregate.Factory>();
            var withinOneHour = new TimeRange(clock.Now, TimeSpan.FromHours(1));
            var withinTwoHours = new TimeRange(clock.Now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            return factory.New(
                pickupAddress: "right here",
                pickupTime: withinOneHour,
                dropoffAddress: "over there",
                dropoffTime: withinTwoHours,
                otherTerms: "other terms",
                biddingAllowed: nextFiveMinutes);
        }

        [Test]
        public void OptimisticConcurrencyControl()
        {
            // Arrange
            var container1 = Container.GetNestedContainer();
            var session1 = container1.GetInstance<ISession>();
            var repository1 = container1.GetInstance<IReverseAuctionRepository>();

            var container2 = Container.GetNestedContainer();
            var session2 = container2.GetInstance<ISession>();
            var repository2 = container2.GetInstance<IReverseAuctionRepository>();

            Assert.AreNotSame(session1, session2);
            Assert.AreNotSame(repository1, repository2);

            var aggregate1 = NewReverseAuction();
            repository1.Save(aggregate1);

            var aggregate2 = repository2.Get(aggregate1.Id);
            Assert.AreNotSame(aggregate1, aggregate2);

            // Act
            var mutator = container1.GetInstance<ReverseAuctionAggregate.AlterPickupLocation>();
            mutator.ChangePickup(aggregate2, "Abu Dahbi");
            mutator.ChangePickup(aggregate1, "Timbuktu");
            Console.WriteLine($"aggregate1.Version before save: {aggregate1.Version}");
            Console.WriteLine($"aggregate2.Version before save: {aggregate2.Version}");

            repository1.Save(aggregate1);
            session1.Flush();
            Console.WriteLine($"aggregate1.Version after save: {aggregate1.Version}");

            // Assert
            repository2.Save(aggregate2);
            Assert.Throws<StaleObjectStateException>(() =>
            {
                session2.Flush();
            });
        }
    }
}
