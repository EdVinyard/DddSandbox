using System;
using System.Collections;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD;
using Domain.Aggregate.Auction;
using Domain.Aggregate.Auction.Event;
using Domain.Aggregate.Common;
using Domain.Port;
using NUnit.Framework;
using StructureMap;

namespace DomainTest
{
    [TestFixture]
    public class ReverseAuctionAlterPickupTest : DomainTest
    {
        [Test]
        public void ShouldChangePickupLocation()
        {
            // Arrange
            var geocoder = (FakeGeocoder)Container.GetInstance<IGeocoder>();
            geocoder["original"] = Austin;
            geocoder["changed"] = Seattle;
            var aggregate = NewAuctionWithPickup("original");

            // Act
            aggregate.AlterPickup(
                Container.GetInstance<IDependencies>(), 
                "changed");

            // Assert
            Assert.AreEqual(
                "changed", 
                aggregate.BuyerTerms.Pickup.Place.Address);
            Assert.AreEqual(
                geocoder["changed"], 
                aggregate.BuyerTerms.Pickup.Place.Coordinates);
        }

        [Test]
        public void ShouldPublishBuyerTermsChangedEvent()
        {
            // Arrange
            var bus = (FakeEventBus)Container.GetInstance<IInterAggregateEventBus>();
            var geocoder = (FakeGeocoder)Container.GetInstance<IGeocoder>();
            geocoder["original"] = Austin;
            geocoder["changed"] = Seattle;
            var aggregate = NewAuctionWithPickup("original");

            // Act
            aggregate.AlterPickup(
                Container.GetInstance<IDependencies>(),
                "changed");

            // Assert
            var lastEvent = bus.EventLog.Last();
            Assert.IsAssignableFrom<BuyerTermsChanged>(lastEvent);

            var termsChanged = (BuyerTermsChanged)lastEvent;
            Assert.AreEqual(aggregate.Id, termsChanged.Id);
        }

        private ReverseAuctionAggregate NewAuctionWithPickup(string pickupLocation)
        {
            var geocoder = (FakeGeocoder)Container.GetInstance<IGeocoder>();
            geocoder["dropoff"] = KansasCity;

            var now = Container.GetInstance<IClock>().Now;
            var withinOneHour = new TimeRange(now, TimeSpan.FromHours(1));
            var withinTwoHours = new TimeRange(now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(now, TimeSpan.FromMinutes(5));

            var factory = Container.GetInstance<ReverseAuctionAggregate.Factory>();

            return factory.New(
                pickupAddress: pickupLocation,
                pickupTime: withinOneHour,
                dropoffAddress: "dropoff",
                dropoffTime: withinTwoHours,
                otherTerms: "other terms",
                biddingAllowed: nextFiveMinutes);
        }

        protected override void Configure(ConfigurationExpression c)
        {
            c.For<IClock>().Use<FakeClock>();
            c.For<IGeocoder>().Use<FakeGeocoder>();
            c.For<IInterAggregateEventBus>().Use<FakeEventBus>();
        }

        private static readonly GeoCoordinate Austin = 
            new GeoCoordinate { Latitude = 30, Longitude = -97 };
        private static readonly GeoCoordinate Seattle = 
            new GeoCoordinate { Latitude = 47, Longitude = -122 };
        private static readonly GeoCoordinate KansasCity =
            new GeoCoordinate { Latitude = 39, Longitude = -95 };

        private class FakeGeocoder : Dictionary<string, GeoCoordinate>, IGeocoder
        {
            public GeoCoordinate GeoCode(string address) => this[address];
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
                Console.WriteLine($"Registered subscription for {typeof(T).Name}.");
            }
        }
    }
}
