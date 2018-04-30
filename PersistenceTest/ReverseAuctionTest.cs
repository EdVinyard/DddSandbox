using DDD;
using Domain.Aggregate.Auction;
using Domain.Aggregate.Common;
using Domain.Port;
using NHibernate;
using NUnit.Framework;
using StructureMap;
using System;
using System.Device.Location;

namespace PersistenceTest
{
    [TestFixture]
    public class ReverseAuctionTest : DatabaseTest
    {
        public class RandomGeocoder : IGeocoder
        {
            private static readonly Random _prng = new Random();
            public GeoCoordinate GeoCode(string address)
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

        public class StructureMapAdapter : IDependencies
        {
            private readonly IContainer c;
            public StructureMapAdapter(IContainer c) { this.c = c; }
            public T Instance<T>() where T : DDD.HasDependencies => c.GetInstance<T>();
        }

        protected override void Configure(ConfigurationExpression c)
        {
            c.For<IClock>().Use<FakeClock>();
            c.For<IGeocoder>().Use<RandomGeocoder>();
            c.For<IInterAggregateEventBus>().Use<FakeEventBus>();
            c.For<IDependencies>().Use<StructureMapAdapter>();
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

        // Sometimes not all tests run and the following error occurs:
        //
        //    Could not find test executor with URI 'executor://nunit3testexecutor/'.
        //
        // Read https://github.com/nunit/nunit3-vs-adapter/issues/399, but
        // no resolution so far.  The .CONFIG files mentioned in that thread 
        // are in
        //
        //    C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow
        //
        // This doesn't happen from the command line.  See RunAllTests.ps1.

        [Test]
        public void OptimisticConcurrencyControl()
        {
            // Arrange
            var ann = new ConcurrentPickupAddressModifier(
                Container.GetNestedContainer(),
                "Ann");
            var bob = new ConcurrentPickupAddressModifier(
                Container.GetNestedContainer(),
                "Bob");
            Assert.That(ann.IsOnSeparateNHibernateSessionFrom(bob));

            var id = ann.Save(NewReverseAuction());
            bob.LoadExistingReverseAuction(id);

            // Act
            ann.AlterPickupButDoNotFlush("Abu Dahbi");
            bob.AlterPickupButDoNotFlush("Timbuktu");

            ann.FlushNHibernateSession();
            // Notice that Ann just saved version 2, but Bob has version 1!

            // Assert
            Assert.Throws<StaleObjectStateException>(() =>
            {
                bob.FlushNHibernateSession();
            });
        }

        /// <summary>
        /// A DSL to make the concurrency test more readable.
        /// </summary>
        private class ConcurrentPickupAddressModifier
        {
            private string _label;
            private IContainer _container;
            private ISession _session;
            private IReverseAuctionRepository _repository;
            private IDependencies _di;
            public ReverseAuctionAggregate _aggregate;

            public ConcurrentPickupAddressModifier(IContainer c, string label)
            {
                _label = label;
                _container = c;
                _session = c.GetInstance<ISession>();
                _repository = c.GetInstance<IReverseAuctionRepository>();
                _di = c.GetInstance<IDependencies>();
                _aggregate = null;
            }

            public bool IsOnSeparateNHibernateSessionFrom(ConcurrentPickupAddressModifier other)
            {
                return !object.ReferenceEquals(_session, other._session)
                    && !object.ReferenceEquals(_repository, other._repository);
            }

            public int Save(ReverseAuctionAggregate agg)
            {
                _aggregate = agg;
                _repository.Save(_aggregate);
                return _aggregate.Id;
            }

            public void LoadExistingReverseAuction(int id)
            {
                _aggregate = _repository.Get(id);
            }

            public void AlterPickupButDoNotFlush(string newPickup)
            {
                _aggregate.AlterPickup(_di, "Timbuktu");
                Console.WriteLine($"{_label}.Version before save: {_aggregate.Version}");
                _repository.Save(_aggregate);
            }

            public void FlushNHibernateSession()
            {
                _session.Flush();
                Console.WriteLine($"{_label}.Version after save: {_aggregate.Version}");
            }
        }
    }
}
