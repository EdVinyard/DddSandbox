using System;
using System.Collections;
using System.Linq;

namespace DomainDrivenDesignTest
{
    public abstract class AllDomainTypes : IEnumerable
    {
        // TODO: What if the Domain is split up into separate assemblies?
        protected static readonly Type[] DomainTypes = 
            typeof(Domain.AssemblyMarker)
            .Assembly
            .GetTypes()
            .ToArray();

        public abstract IEnumerator GetEnumerator();
    }

    public class AllDomainEntities : AllDomainTypes
    {
        protected static readonly Type[] Entities = 
            DomainTypes
            .Where(t => typeof(DDD.Entity).IsAssignableFrom(t))
            .ToArray();

        public override IEnumerator GetEnumerator() => Entities.GetEnumerator();
    }

    public class AllDomainValueTypes : AllDomainTypes
    {
        protected static readonly Type[] ValueTypes = 
            DomainTypes
            .Where(t => typeof(DDD.ValueType).IsAssignableFrom(t))
            .ToArray();

        public override IEnumerator GetEnumerator() => ValueTypes.GetEnumerator();
    }
}
