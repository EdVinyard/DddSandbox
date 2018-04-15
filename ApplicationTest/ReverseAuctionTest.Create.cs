using Application;
using Framework;
using NUnit.Framework;
using StructureMap;
using Repr = Application.Representation;

namespace ApplicationTest.ReverseAuctionTest
{
    [TestFixture]
    public class Create : IntegrationTest
    {
        private FakeClock _fakeClock = new FakeClock();

        protected override void Configure(ConfigurationExpression c)
        {
            c.For<DDD.IInterAggregateEventBus>().Use<FakeEventBus>();
            c.For<Domain.Port.IDependencies>().Use<StructureMapAdapter>();
            c.For<Domain.Port.IGeocoder>().Use<FakeGeocoder>();
            c.For<Domain.Port.IClock>().Use(_fakeClock);
        }

        [Test]
        public void SmokeTest()
        {
            // Arrange
            var now              = _fakeClock.Now              .ToIso8601();
            var fiveMinutesHence = _fakeClock.Now.AddMinutes(5).ToIso8601();
            var oneWeekHence     = _fakeClock.Now.AddDays(7)   .ToIso8601();

            var expected = new Repr.ReverseAuction
            {
                OtherTerms = "a skateboard",

                Pickup = new Repr.Waypoint
                {
                    Address = "742 Evergreen Terr, Springfield, IL 62704",
                    Earliest = now,
                    Latest = oneWeekHence,
                },

                Dropoff = new Repr.Waypoint
                {
                    Address = "205 Brazos St, Austin, TX 78701",
                    Earliest = now,
                    Latest = oneWeekHence,
                },

                BiddingStart = now,
                BiddingEnd = fiveMinutesHence,
            };

            // Act
            var create = Container.GetInstance<ReverseAuction.Create>();
            var actual = create.From(expected);

            // Assert
            Assert.AreEqual(expected.OtherTerms, actual.OtherTerms);

            Assert.AreEqual(expected.Pickup.Address, actual.Pickup.Address);
            Assert.AreNotEqual(default(double), actual.Pickup.Latitude);
            Assert.AreNotEqual(default(double), actual.Pickup.Longitude);
            Assert.AreEqual(expected.Pickup.Earliest, actual.Pickup.Earliest);
            Assert.AreEqual(expected.Pickup.Latest, actual.Pickup.Latest);

            Assert.AreEqual(expected.Dropoff.Address, actual.Dropoff.Address);
            Assert.AreNotEqual(default(double), actual.Dropoff.Latitude);
            Assert.AreNotEqual(default(double), actual.Dropoff.Longitude);
            Assert.AreEqual(expected.Dropoff.Earliest, actual.Dropoff.Earliest);
            Assert.AreEqual(expected.Dropoff.Latest, actual.Dropoff.Latest);

            Assert.AreEqual(expected.BiddingStart, actual.BiddingStart);
            Assert.AreEqual(expected.BiddingEnd, actual.BiddingEnd);
        }
    }
}
