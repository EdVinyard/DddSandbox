using Domain.Port;
using System;
using System.Collections.Generic;
using System.Device.Location;

namespace Domain.Aggregate.Auction
{
    /// <summary>
    /// A place on Earth, with a colloquial name (Address) and precise formal 
    /// specification (GeoCoordinate).
    /// </summary>
    public class Location : ValueType
    {
        // We could create a constructor here, but if instantiation
        // requires dependencies outside this Domain Type, it's better
        // to put *all* the preconditions, validation, and construction
        // logic in one place, the Location.Constructor Domain Service,
        // so there's no ambiguity about where those checks and details
        // belong.
        //
        // private Location(
        //     string address,
        //     GeoCoordinate coordinates) 
        // {
        //     if (null == coordinates) 
        //     {
        //         throw new ArgumentNullException("coordinates");
        //     }
        //     ...
        // }
        //
        // Structuring a Domain Service this way has advantages:
        // - unlike a separate class, it's more discoverable: the Domain 
        //   Type Address is a namespace for all Address-specific Domain services
        // - unlike a Domain class method, may have injected dependencies
        // - tighter encapsulation of Domain class methods; members may remain
        //   protected or private but are still accessible to this process
        //
        // You could name it "Creator", "Factory", or whatever makes you happy
        // instead of "Constructor".  Picking a single name to stick with 
        // everywhere in your code is way more important than the one you
        // pick.

        /// <summary>
        /// A stateless Domain Service that creates instances of the Address
        /// Domain Entity.
        /// <summary>
        public class Constructor : Service
        {
            private readonly IGeocoder _geocoder;

            public Constructor(IGeocoder geocoder)
            {
                _geocoder = geocoder;
            }

            public Location New(string address)
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    throw new ArgumentException(
                        "wholeAddress cannot be null, empty, or whitespace.",
                        "wholeAddress");
                }

                var coordinates = _geocoder.GeoCode(address);
                // Here, perhaps require that these be coordiantes of some quality,
                // precision, or within some service area.

                return new Location
                {
                    Address = address,
                    Coordinates = coordinates,
                };
            }
        }

        /// <summary>
        /// FOR NHibernate and Location.Constructor ONLY!
        /// </summary>
        protected Location() { }

        /// <summary>
        /// A Location that does not exist.  Prefer this to allowing <c>null</c>
        /// instances of Location whenever possible.
        /// </summary>
        public static readonly Location Nowhere = new Location {
            Address = "nowhere",
            Coordinates = GeoCoordinate.Unknown,
        };

        /// <summary>
        /// The text by which a human would identify (and communicate) the
        /// identity of this location.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// The latitude-longitude coordinates of this location.
        /// </summary>
        public GeoCoordinate Coordinates { get; private set; }

        public override bool Equals(object otherObj)
        {
            if (null == otherObj) return false;
            if (ReferenceEquals(this, otherObj)) return true;
            var otherLocation = otherObj as Location;
            if (null == otherLocation) return false;
            return this.Equals(otherLocation);
        }

        public bool Equals(Location other)
        {
            return (Address == other.Address)
                && (Coordinates == other.Coordinates);
        }

        public static bool operator ==(Location l, Location r) => Object.Equals(l, r);
        public static bool operator !=(Location l, Location r) => !(l == r);

        public override int GetHashCode()
        {
            var hashCode = 2005225651;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + EqualityComparer<GeoCoordinate>.Default.GetHashCode(Coordinates);
            return hashCode;
        }
    }
}
