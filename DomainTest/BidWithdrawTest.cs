using DDD;
using Domain.Aggregate.Auction;
using Domain.Aggregate.Bid;
using Domain.Aggregate.Bid.Event;
using Domain.Aggregate.Common;
using Domain.Port;
using Framework;
using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace DomainTest
{
    [TestFixture]
    public class BidAggregateWithdrawTest : DomainTest
    {
        protected override void ArrangeDependencies(ConfigurationExpression x)
        {
            x.For<IInterAggregateEventBus>().Use<FakeEventBus>();
            x.For<IClock>().Use<FakeClock>();
            x.For<IGeocoder>().Use<FakeRandomGeocoder>();
        }

        [Test]
        public void ShouldMarkBidAsWithdrawn()
        {
            // Arrange
            var bid = NewBid();
            Assert.IsTrue(bid.IsTendered);
            Assert.IsFalse(bid.IsWithdrawn);

            // Act
            bid.Withdraw();

            // Assert
            Assert.IsTrue(bid.IsWithdrawn);
            Assert.IsFalse(bid.IsWithdrawn);
        }

        [Test]
        public void ShouldPublishEvent()
        {
            // At the Application level, we'll test the effect of this event 
            // being published and then consumed by the AuctionAggregate.  
            // Here, we're only testing within the confines of the 
            // BidAggregate.

            // Arrange
            var bid = NewBid();
            var bus = (FakeEventBus)Container.GetInstance<IInterAggregateEventBus>();

            // Act
            bid.Withdraw();

            // Assert
            var lastEvent = bus.EventLog.Last();
            Assert.IsAssignableFrom<BidWithdrawn>(lastEvent);

            var withdrawnEvent = (BidWithdrawn)lastEvent;
            Assert.AreEqual(bid.Id, withdrawnEvent.Id);
            Assert.AreEqual(bid.Price, withdrawnEvent.Price);
        }

        [Test]
        public void WhenBidWasAlreadyWithdrawnShouldRemainWithdrawn()
        {
            // Arrange
            var bid = NewBid();
            bid.Withdraw();

            Assert.IsTrue(bid.IsWithdrawn);
            Assert.IsFalse(bid.IsWithdrawn);

            // Act
            bid.Withdraw();

            // Assert
            Assert.IsTrue(bid.IsWithdrawn);
            Assert.IsFalse(bid.IsWithdrawn);
        }

        [Test]
        public void WhenBidWasAlreadyWithdrawnShouldNotPublishEvent()
        {
            // Arrange
            var bus = (FakeEventBus)Container.GetInstance<IInterAggregateEventBus>();
            var bid = NewBid();
            bid.Withdraw();
            bus.EventLog.Clear();

            // Act
            bid.Withdraw();

            // Assert
            Assert.IsTrue(bus.EventLog.IsEmpty());
        }

        private BidAggregate NewBid()
        {
            // TODO: This really seems like the "long way 'round".  We need
            // all this mechanism just to get an instance of a Bid.  That's
            // a good thing because it means that we're following all the
            // rules that the Application layer would.  It's a bad thing
            // because it complicates the test substantially.  Which one is
            // more important?  Can we have our cake and eat it, too?

            var now = Container.GetInstance<IClock>().Now;
            var withinOneHour = new TimeRange(now, TimeSpan.FromHours(1));
            var withinTwoHours = new TimeRange(now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(now, TimeSpan.FromMinutes(5));

            var factory = Container.GetInstance<ReverseAuctionAggregate.Factory>();

            var auction = factory.New(
                pickupAddress: "pickup",
                pickupTime: withinOneHour,
                dropoffAddress: "dropoff",
                dropoffTime: withinTwoHours,
                otherTerms: "other terms",
                biddingAllowed: nextFiveMinutes);

            return auction.PlaceBid(
                Container.GetInstance<IDependencies>(),
                pickupTime: auction.BuyerTerms.Pickup.Time,
                dropoffTime: auction.BuyerTerms.Dropoff.Time,
                price: Money.USD(123.45m));
        }

        private class FakeEventBus : IInterAggregateEventBus
        {
            public List<InterAggregateEvent> EventLog =
                new List<InterAggregateEvent>();

            public void Publish(InterAggregateEvent anEvent)
            {
                EventLog.Add(anEvent);
                Console.WriteLine($"Published: {anEvent}");
            }

            public void Subscribe<T>(Action<T> subscriber) where T : InterAggregateEvent
            {
                throw new NotImplementedException();
            }
        }

        public class FakeClock : IClock
        {
            public FakeClock()
            {
                Now = new DateTimeOffset(
                    2018, 2, 3,
                    4, 13, 59,
                    TimeSpan.FromHours(1));
            }

            public DateTimeOffset Now { get; set; }
        }

        public class FakeRandomGeocoder : IGeocoder
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
    }
}
