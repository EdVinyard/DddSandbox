using StructureMap;

namespace Application
{
    public class StructureMapAdapter : Domain.Port.IDependencies
    {
        private readonly IContainer c;
        public StructureMapAdapter(IContainer c) { this.c = c; }
        public T Instance<T>() where T : DDD.HasDependencies {
            return c.GetInstance<T>();
        }
    }
}
