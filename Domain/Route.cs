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
        public TimeRange Time { get; private set; }

        /// <summary>
        /// A place that is part of a 
        /// </summary>
        public Location Place { get; private set; }

        public Waypoint(Location place, TimeRange time)
        {
            Place = place;
            Time = time;
        }
    }

    /// <summary>
    /// An ordered sequence of Waypoints that signify a trip with a beginning,
    /// (optionally) some significant intermediate stops/milestones, and a
    /// destination.
    /// </summary>
    public class Route
    {
        public virtual int RouteId { get; protected set; }

        private string _label;
        public virtual string Label
        {
            get { return _label; }
            protected set { _label = value ?? string.Empty; }
        }

        private IReadOnlyList<Waypoint> _waypoints;
        public virtual IReadOnlyList<Waypoint> Waypoints
        {
            get { return _waypoints; }
            protected set { _waypoints = value ?? new List<Waypoint>(0); }
        }

        /// <summary>
        /// FOR NHibernate USE ONLY!
        /// </summary>
        protected Route() : this(null, null) { }

        public Route(
            string label,
            IEnumerable<Waypoint> waypoints)
        {
            _label = label ?? string.Empty;
            _waypoints = new List<Waypoint>(waypoints ?? new Waypoint[0]);
        }
    }
}
