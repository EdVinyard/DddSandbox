using Application;
using Domain.Aggregate.Auction;
using NUnit.Framework;
using StructureMap;
using System;

namespace ApplicationTest
{
    /// <summary>
    /// An Application test that includes the Domain and real implementations of 
    /// Repositories and Application services.  This is *almost* an end-to-end
    /// test, but it OMITS things that provided by the UserInterface (e.g.,
    /// an implementation of IUriScheme), and things that would tempt us to 
    /// introduce timing delays or waits into our tests (time, external web 
    /// services).
    /// </summary>
    [NUnit.Framework.TestFixture]
    public abstract class IntegrationTest
    {
        static IntegrationTest()
        {
            globalContainer = new Container(c =>
            {
                // NOTICE: DO NOT run the Application.DependencyRegistry here
                // because those services are often either introduce testing challenges
                // (e.g., IClock) or network traffic we can't tightly control (e.g.,
                // IGeoCoder).  When those services are needed, individual tests must
                // decide how best to include or fake them.

                c.AddRegistry<Persistence.DependencyRegistry>();
                c.For<Application.IUriScheme>().Use<FakeUriScheme>();
            });
        }
        /// <summary>
        /// a root-level DI Container, constructed once for ALL instances of
        /// DatabaseTest
        /// </summary>
        private static IContainer globalContainer;

        /// <summary>
        /// a nested Container, constructed for each test method
        /// see http://structuremap.github.io/the-container/nested-containers/
        /// </summary>
        protected IContainer Container { get; private set; }

        [SetUp]
        public void PerTestSetUp()
        {
            Container = globalContainer.GetNestedContainer();
            Container.Configure(x =>
            {
                x.For<NHibernate.ISession>().Use(ctx =>
                    ctx.GetInstance<NHibernate.ISessionFactory>().OpenSession());
                Configure(x);
            });
        }

        /// <summary>
        /// configure the per-test, nested DI Container
        /// </summary>
        protected abstract void Configure(ConfigurationExpression c);

        private class FakeUriScheme : IUriScheme
        {
            public Uri ToUri(ReverseAuctionAggregate reverseAuction)
            {
                return new Uri($"http://example.com/ReverseAuction/{reverseAuction.Id}");
            }
        }
    }
}
