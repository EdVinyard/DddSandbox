using StructureMap;

namespace UserInterface
{
    public class DependencyRegistry : Registry
    {
        public DependencyRegistry() : base()
        {
            For<Application.IUriScheme>().Use<WebApiUriScheme>();
        }
    }
}
