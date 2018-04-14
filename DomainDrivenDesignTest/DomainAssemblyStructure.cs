using NUnit.Framework;
using System;

namespace DomainDrivenDesignTest
{
    [TestFixture]
    public class DomainAssemblyStructure
    {
        [TestCaseSource(typeof(AllDomainTypes))]
        public void AllTypesInDomainAssemblyMustHaveDddRole(Type typeInDomainAssembly)
        {
            Assert.IsTrue(typeInDomainAssembly.Implements<DDD._Marker>());
        }
    }
}
