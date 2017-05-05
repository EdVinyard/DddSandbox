using System.Collections.Generic;
using System.Device.Location;

namespace Domain
{
    /// <summary>
    /// A place on Earth, with a colloquial name (Address) and precise formal 
    /// specification (GeoCoordinate).
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The text by which a human would identify (and communicate) the
        /// identity of this location.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// The latitude-longitude coordinates of this location.
        /// </summary>
        public GeoCoordinate GeoCoordinate { get; private set; }

        public Location(string address, GeoCoordinate geoCoordinate)
        {
            Address = address;
            GeoCoordinate = geoCoordinate;
        }
    }

    /// <summary>
    /// A time and place that denote one point of a trip: the starting point,
    /// stopping point, or some significant intermediate point between.
    /// </summary>
    public class Waypoint
    {
        /// <summary>
        /// The range of time during which the Location must be reached.
        /// </summary>
        public TimeRange TimeRange { get; private set; }

        /// <summary>
        /// A place that is part of a 
        /// </summary>
        public Location Location { get; private set; }

        public Waypoint(Location l, TimeRange t)
        {
            Location = l;
            TimeRange = t;
        }
    }

    /// <summary>
    /// An ordered sequence of Waypoints that signify a trip with a beginning,
    /// (optionally) some significant intermediate stops/milestones, and a
    /// destination.
    /// </summary>
    public class Route
    {
        public IReadOnlyList<Waypoint> Waypoints { get; private set; }

        public Route(IEnumerable<Waypoint> waypoints)
        {
            Waypoints = new List<Waypoint>(waypoints);
        }
    }
}
