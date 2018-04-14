using Domain.Aggregate.Common;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Aggregate.Auction //: Aggregate
{
    /// <summary>
    /// A time and place that denote one point of a trip: the starting point,
    /// stopping point, or some significant intermediate point between.
    /// </summary>
    public class Waypoint : DDD.ValueType, IEquatable<Waypoint>
    {
        private TimeRange _time;
        private Location _place;

        /// <summary>
        /// The range of time during which the Location must be reached.
        /// </summary>
        public virtual TimeRange Time {
            get { return _time; }
            protected set { _time = value; }
        }

        /// <summary>
        /// A place that is part of a 
        /// </summary>
        public virtual Location Place {
            get { return _place; }
            protected set { _place = value; }
        }

        /// <summary>
        /// FOR NHibernate and Terms.Constructor ONLY!
        /// </summary> 
        protected Waypoint() : this(Location.Nowhere, TimeRange.Never) { }

        public Waypoint(Location place, TimeRange time)
        {
            Precondition.MustNotBeNull(place, nameof(place));
            Precondition.MustNotBeNull(time, nameof(time));

            _place = place;
            _time = time;
        }

        public Waypoint(TimeRange time, Location place)
        {
            _time = time;
            _place = place;
        }

        public override bool Equals(object otherObj)
        {
            if (null == otherObj) return false;
            if (ReferenceEquals(this, otherObj)) return true;
            var otherTerms = otherObj as Waypoint;
            if (null == otherTerms) return false;
            return this.Equals(otherTerms);
        }

        public bool Equals(Waypoint other)
        {
            return (Time == other.Time)
                && (Place == other.Place);
        }

        public static bool operator==(Waypoint l, Waypoint r) => Object.Equals(l, r);
        public static bool operator!=(Waypoint l, Waypoint r) => !(l == r);

        public override int GetHashCode()
        {
            var hashCode = -2104346736;
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeRange>.Default.GetHashCode(_time);
            hashCode = hashCode * -1521134295 + EqualityComparer<Location>.Default.GetHashCode(_place);
            return hashCode;
        }
    }
}
