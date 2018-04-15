using NHibernate;
using NUnit.Framework;
using StructureMap;

namespace PersistenceTest
{
    [NUnit.Framework.TestFixture]
    public abstract class DatabaseTest
    {
        static DatabaseTest()
        {
            globalContainer = new Container(x =>
            {
                x.AddRegistry<Persistence.DependencyRegistry>();
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

        protected ISession Session => Container.GetInstance<ISession>();
 
        [SetUp]
        public void PerTestSetUp()
        {
            Container = globalContainer.GetNestedContainer();
            Container.Configure(x =>
            {
                x.For<ISession>().Use(c => c.GetInstance<ISessionFactory>().OpenSession());
                Configure(x);
            });
        }

        /// <summary>
        /// configure the per-test, nested DI Container
        /// </summary>
        protected abstract void Configure(ConfigurationExpression c);
    }
}
