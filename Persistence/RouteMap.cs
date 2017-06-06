using Domain;
using FluentNHibernate.Mapping;

namespace Persistence
{
    internal class RouteMap : ClassMap<Route>
    {
        public RouteMap()
        {
            Id(x => x.RouteId);
            Map(x => x.Label);
        }
    }
}