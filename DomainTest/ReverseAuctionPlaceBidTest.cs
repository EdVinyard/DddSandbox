﻿using Domain;
using Domain.Aggregate.Auction;
using Domain.Aggregate.Common;
using Domain.Port;
using NUnit.Framework;
using StructureMap;
using System;
using System.Device.Location;

namespace DomainTest
{
    [TestFixture]
    public class ReverseAuctionPlaceBidTest : DomainTest
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
            public FakeClock()
            {
                Now = new DateTimeOffset(
                    2018, 2, 3,
                    4, 13, 59,
                    TimeSpan.FromHours(1));
            }

            public DateTimeOffset Now { get; set; }
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
        }

        [Test]
        public void BiddingIsAllowedNow()
        {
            // Arrange
            var clock = Container.GetInstance<IClock>();
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            var auction = NewAuction(biddingAllowed: nextFiveMinutes);

            // Act and Assert
            Assert.DoesNotThrow(() => auction.PlaceBid(
                Container.GetInstance<IDependencies>(),
                auction.BuyerTerms.Pickup.Time,
                auction.BuyerTerms.Dropoff.Time,
                Money.USD(1.00m)));
        }

        [Test]
        public void BiddingIsProhibitedNow()
        {
            // Arrange
            var clock = (FakeClock)Container.GetInstance<IClock>();
            var originalTime = clock.Now;
            var fiveMinutesAgo = clock.Now.AddMinutes(-5);
            var lastFiveMinutes = new TimeRange(fiveMinutesAgo, TimeSpan.FromMinutes(5));

            clock.Now = fiveMinutesAgo;
            var auction = NewAuction(biddingAllowed: lastFiveMinutes);
            clock.Now = originalTime;

            // Act and Assert
            Assert.Throws<ReverseAuctionAggregate.BiddingNotAllowedNow>(
                () => auction.PlaceBid(
                    Container.GetInstance<IDependencies>(),
                    auction.BuyerTerms.Pickup.Time,
                    auction.BuyerTerms.Dropoff.Time,
                    Money.USD(1.00m)));
        }

        private ReverseAuctionAggregate NewAuction(TimeRange biddingAllowed)
        {
            var clock = Container.GetInstance<IClock>();
            var factory = Container.GetInstance<ReverseAuctionAggregate.Factory>();
            var withinOneHour = new TimeRange(clock.Now, TimeSpan.FromHours(1));
            var withinTwoHours = new TimeRange(clock.Now, TimeSpan.FromHours(2));

            return factory.New(
                pickupAddress: "right here",
                pickupTime: withinOneHour,
                dropoffAddress: "over there",
                dropoffTime: withinTwoHours,
                otherTerms: "other terms",
                biddingAllowed: biddingAllowed);
        }

        [Test]
        public void BidPickupTimeDisagreesWithAuctionPickupTime()
        {
            // Arrange
            var clock = Container.GetInstance<IClock>();
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            var auction = NewAuction(biddingAllowed: nextFiveMinutes);
            var pickupTimeThatDisagrees = new TimeRange(
                auction.BuyerTerms.Pickup.Time.Start,
                auction.BuyerTerms.Pickup.Time.Duration.Add(TimeSpan.FromMinutes(1)));

            // Act and Assert
            Assert.Throws<ReverseAuctionAggregate.BidTimeDisagreesWithAuctionTime>(
                () => auction.PlaceBid(
                    Container.GetInstance<IDependencies>(),
                    pickupTimeThatDisagrees,
                    auction.BuyerTerms.Dropoff.Time,
                    Money.USD(1.00m)));
        }

        [Test]
        public void BidPickupTimeAgreesWithAuctionPickupTime()
        {
            // Arrange
            var clock = Container.GetInstance<IClock>();
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            var auction = NewAuction(biddingAllowed: nextFiveMinutes);
            var pickupTimeThatAgrees = new TimeRange(
                auction.BuyerTerms.Pickup.Time.Start,
                auction.BuyerTerms.Pickup.Time.Duration.Add(TimeSpan.FromMinutes(-1)));

            // Act and Assert
            Assert.DoesNotThrow(() => auction.PlaceBid(
                Container.GetInstance<IDependencies>(),
                pickupTimeThatAgrees,
                auction.BuyerTerms.Dropoff.Time,
                Money.USD(1.00m)));
        }

        [Test]
        public void BidDropoffTimeDisagreesWithAuctionDropoffTime()
        {
            // Arrange
            var clock = Container.GetInstance<IClock>();
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            var auction = NewAuction(biddingAllowed: nextFiveMinutes);
            var dropoffTimeThatDisagrees = new TimeRange(
                auction.BuyerTerms.Dropoff.Time.Start,
                auction.BuyerTerms.Dropoff.Time.Duration.Add(TimeSpan.FromMinutes(1)));

            // Act and Assert
            Assert.Throws<ReverseAuctionAggregate.BidTimeDisagreesWithAuctionTime>(
                () => auction.PlaceBid(
                    Container.GetInstance<IDependencies>(),
                    auction.BuyerTerms.Pickup.Time,
                    dropoffTimeThatDisagrees,
                    Money.USD(1.00m)));
        }

        [Test]
        public void BidDropoffTimeAgreesWithAuctionDropoffTime()
        {
            // Arrange
            var clock = Container.GetInstance<IClock>();
            var nextFiveMinutes = new TimeRange(clock.Now, TimeSpan.FromMinutes(5));

            var auction = NewAuction(biddingAllowed: nextFiveMinutes);
            var dropoffTimeThatAgrees = new TimeRange(
                auction.BuyerTerms.Dropoff.Time.Start,
                auction.BuyerTerms.Dropoff.Time.Duration.Add(TimeSpan.FromMinutes(-1)));

            // Act and Assert
            Assert.DoesNotThrow(() => auction.PlaceBid(
                Container.GetInstance<IDependencies>(),
                auction.BuyerTerms.Pickup.Time,
                dropoffTimeThatAgrees,
                Money.USD(1.00m)));
        }
    }
}
