using System;
using System.Collections;
using System.Linq;

namespace DomainDrivenDesignTest
{
    public class DomainTypes : IEnumerable
    {
        // TODO: What if the Domain is split up into separate assemblies?
        internal static readonly Type[] All = 
            typeof(Domain.AssemblyMarker)
            .Assembly
            .GetTypes()
            .ToArray();

        public IEnumerator GetEnumerator() => All.GetEnumerator();
    }

    public class AllDomainEntities : IEnumerable
    {
        internal static readonly Type[] Entities = 
            DomainTypes.All
            .Where(t => typeof(DDD.Entity).IsAssignableFrom(t))
            .ToArray();

        public IEnumerator GetEnumerator() => Entities.GetEnumerator();
    }

    public class AllDomainValueTypes : IEnumerable
    {
        internal static readonly Type[] ValueTypes = 
            DomainTypes.All
            .Where(t => typeof(DDD.ValueType).IsAssignableFrom(t))
            .ToArray();

        public IEnumerator GetEnumerator() => ValueTypes.GetEnumerator();
    }
}
