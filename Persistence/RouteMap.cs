using Domain;
using FluentNHibernate.Mapping;
using System.Device.Location;

namespace Persistence
{
    internal class TimeRangeMap : ComponentMap<TimeRange>
    {
        public TimeRangeMap()
        {
            Map(x => x.Start).Column("Early");
            Map(x => x.End).Column("Late");
        }
    }

    internal class RouteMap : ClassMap<Route>
    {
        public RouteMap()
        {
            Id(x => x.RouteId);
            Map(x => x.Label);
        }
    }

    internal class WaypointMap : ClassMap<Waypoint>
    {
        public WaypointMap()
        {
            Id(x => x.WaypointId);
        }
    }
}