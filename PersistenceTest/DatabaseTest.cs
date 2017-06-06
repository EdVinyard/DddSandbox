using NUnit.Framework;

namespace PersistenceTest
{
    [NUnit.Framework.TestFixture]
    public abstract class DatabaseTest
    {
        protected NHibernate.ISession DbSession { get; private set; }

        [OneTimeSetUp]
        public void DatabaseTestFixtureSetUp()
        {
            DependencyInjectionContainer.SetUp();
            DbSession = DependencyInjectionContainer.Instance
                .GetInstance<NHibernate.ISession>();
        }
    }
}
