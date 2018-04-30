using System;
using Domain.Port;
using NUnit.Framework;
using StructureMap;

namespace DomainTest
{
    public abstract class DomainTest
    {
        static DomainTest()
        {
            globalContainer = new Container(c =>
            {
                c.For<IDependencies>().Use<StructureMapAdapter>().Transient();
            });
        }

        public class StructureMapAdapter : IDependencies
        {
            private readonly IContainer c;
            public StructureMapAdapter(IContainer c) { this.c = c; }
            public T Instance<T>() where T : DDD.HasDependencies => c.GetInstance<T>();
        }

        /// <summary>
        /// A root-level DI Container, constructed once for ALL instances of
        /// DomainTest; this is a way to optimize the construction of expensive
        /// dependencies within the Domain -- but we really don't want any.
        /// Also, I may get rid of this if I measure the cost of new Container
        /// construction and it's low enough compared to 
        /// Container.GetNestedContainer().
        /// </summary>
        private static IContainer globalContainer;

        /// <summary>
        /// a nested Container, constructed for each test method;
        /// customize the content with the <c>Configure()</c> method;
        /// see http://structuremap.github.io/the-container/nested-containers/;
        /// </summary>
        protected IContainer Container { get; private set; }

        [SetUp]
        public void PerTestSetUp()
        {
            Container = globalContainer.GetNestedContainer();
            Container.Configure(ArrangeDependencies);
        }

        protected abstract void ArrangeDependencies(ConfigurationExpression x);
    }
}
