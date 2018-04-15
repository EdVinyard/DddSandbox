using StructureMap;
using System.Web.Http;

namespace UserInterface
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            // RULE: No auto-magical default routing!
            config.Routes.MapHttpRoute(
                name: "ReverseAuction",
                routeTemplate: "ReverseAuction/{id}",
                defaults: new {
                    controller = "ReverseAuction",
                    id = RouteParameter.Optional
                }
            );
        }
    }
}
