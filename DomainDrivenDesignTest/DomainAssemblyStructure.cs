using NUnit.Framework;
using System;

namespace DomainDrivenDesignTest
{
    [TestFixture]
    public class DomainAssemblyStructure
    {
        [TestCaseSource(typeof(DomainTypes))]
        public void AllTypesInDomainAssemblyMustHaveDddRole(Type t)
        {
            if (t == typeof(Domain.AssemblyMarker)) Assert.Pass();

            Assert.IsTrue(
                t.Implements<DDD.HasState>()
                || t.Implements<DDD.HasDependencies>()
                || t.IsDerivedFrom<Exception>(), 
                @"

All types defined in the Domain project(s) must call out their DDD role by 
implemting one of the DDD marker interfaces (e.g., ValueType, Entity, Factory).
Look in the DomainDrivenDesign project for a complete list.

By requiring that all Domain types specify their role, we can enforce other
structural rules about those roles, and ensure that so-called ""god objects""
don't appear within the Domain.  For example, all DDD types have either
dependencies (Service, Factory, Command, Query) or state (ValueType, Entity, 
AggregateRoot, Aggregate) but never both.  A type in the Domain should never
have *both*, and specifying the role of each type assists both automated code-
review (such as this test) and human reviewers to orient themselves.

                ");
        }
    }
}
