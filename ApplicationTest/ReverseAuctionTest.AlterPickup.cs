using Application;
using Domain.Aggregate.Common;
using Domain.Port;
using Framework;
using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Device.Location;
using DomainAuction = Domain.Aggregate.Auction;
using Repr = Application.Representation;

namespace ApplicationTest.ReverseAuctionTest
{
    [TestFixture]
    public class AlterPickup : IntegrationTest
    {
        protected override void Configure(ConfigurationExpression c)
        {
            c.For<DDD.IInterAggregateEventBus>().Use<FakeEventBus>();
            c.For<Domain.Port.IDependencies>().Use<StructureMapAdapter>();
            c.For<Domain.Port.IGeocoder>().Use<FakeGeocoder>();
            c.For<Domain.Port.IClock>().Use(_fakeClock);
            c.For<DomainAuction.IReverseAuctionRepository>().Use(_fakeRepository);
        }

        private FakeClock _fakeClock = new FakeClock();
        private FakeReverseAuctionRepository _fakeRepository =
            new FakeReverseAuctionRepository();

        [Test]
        public void SmokeTest()
        {
            // Arrange
            var now = _fakeClock.Now.ToIso8601();
            var fiveMinutesHence = _fakeClock.Now.AddMinutes(5).ToIso8601();
            var oneHourHence = _fakeClock.Now.AddHours(1).ToIso8601();

            var geocoder = (FakeGeocoder)Container.GetInstance<IGeocoder>();
            geocoder["original"] = Austin;
            geocoder["changed"] = Seattle;

            var aggregate = NewAuctionWithPickup("original");
            _fakeRepository.Saved = aggregate;

            var expected = new Repr.Waypoint
            {
                Address = "changed",
                Earliest = now,
                Latest = oneHourHence,
            };

            // Act
            var alterPickup = Container.GetInstance<ReverseAuction.AlterPickup>();
            var actual = alterPickup.To(aggregate.Id, expected);

            // Assert
            Assert.AreEqual(
                "changed",
                aggregate.BuyerTerms.Pickup.Place.Address);
            Assert.AreEqual(
                geocoder["changed"],
                aggregate.BuyerTerms.Pickup.Place.Coordinates);
        }

        [Test]
        public void WhenRequestedPickupTimeRangeDiffers()
        {
            // STARTHERE
            Assert.Fail("Write this test.");
        }

        private DomainAuction.ReverseAuctionAggregate NewAuctionWithPickup(
            string pickupLocation)
        {
            var geocoder = (FakeGeocoder)Container.GetInstance<IGeocoder>();
            geocoder["dropoff"] = Austin;

            var now = Container.GetInstance<IClock>().Now;
            var withinOneHour = new TimeRange(now, TimeSpan.FromHours(1));
            var withinTwoHours = new TimeRange(now, TimeSpan.FromHours(2));
            var nextFiveMinutes = new TimeRange(now, TimeSpan.FromMinutes(5));

            var factory = Container
                .GetInstance<DomainAuction.ReverseAuctionAggregate.Factory>();

            return factory.New(
                pickupAddress: pickupLocation,
                pickupTime: withinOneHour,
                dropoffAddress: "dropoff",
                dropoffTime: withinTwoHours,
                otherTerms: "other terms",
                biddingAllowed: nextFiveMinutes);
        }

        private static readonly GeoCoordinate Austin =
            new GeoCoordinate { Latitude = 30, Longitude = -97 };
        private static readonly GeoCoordinate Seattle =
            new GeoCoordinate { Latitude = 47, Longitude = -122 };
        private static readonly GeoCoordinate KansasCity =
            new GeoCoordinate { Latitude = 39, Longitude = -95 };

        private class FakeGeocoder : Dictionary<string, GeoCoordinate>, IGeocoder
        {
            public GeoCoordinate GeoCode(string address)
            {
                try
                {
                    return this[address];
                }
                catch (KeyNotFoundException ex)
                {
                    var locations = string.Join(", ", this.Keys);
                    throw new ArgumentException(
                        $"FakeGeocoder failed to geocode '{address}'; " +
                        $"it only knows about {locations}.", 
                        ex);
                }
            }
        }

        private class FakeReverseAuctionRepository : DomainAuction.IReverseAuctionRepository
        {
            public DomainAuction.ReverseAuctionAggregate Saved { get; set; }

            public DomainAuction.ReverseAuctionAggregate Get(int id)
            {
                if (id == Saved.Id) return Saved;

                throw new ArgumentException($"unknown Id: {id}");
            }

            public IReadOnlyList<Domain.Aggregate.Auction.ReverseAuction> GetLive(
                DateTimeOffset dt, 
                int pageSize, 
                int pageIndex)
            {
                throw new NotImplementedException();
            }

            public DomainAuction.ReverseAuctionAggregate Save(
                DomainAuction.ReverseAuctionAggregate ra)
            {
                throw new NotImplementedException();
            }

            public DomainAuction.ReverseAuctionAggregate Update(
                DomainAuction.ReverseAuctionAggregate ra)
            {
                if (ra.Id == Saved.Id)
                {
                    return ra;
                }

                throw new ArgumentException($"unknown Id: {ra.Id}");
            }
        }
    }
}
