namespace Application
{
    public class DependencyRegistry : StructureMap.Registry
    {
        public DependencyRegistry() : base()
        {
            For<DDD.IInterAggregateEventBus>().Use<FakeEventBus>();
            For<Domain.Port.IClock>().Use<Clock>();
            For<Domain.Port.IDependencies>().Use<StructureMapAdapter>();
            For<Domain.Port.IGeocoder>().Use<FakeGeocoder>();
        }
    }
}
