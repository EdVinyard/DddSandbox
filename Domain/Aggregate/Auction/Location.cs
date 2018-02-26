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
        // RULE: Value Types MAY either be instantiated using a traditional
        // constructor, or a class-scoped Factory, whichever is appropriate.
        // Choose a Factory when construction has external dependencies.
        // In the case of Location, an external Geocoder is needed to find
        // coorindates given a human-readable street address.

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
        public class Factory : Service
        {
            private readonly IGeocoder _geocoder;

            public Factory(IGeocoder geocoder)
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
                // This is a relatively simple implementation, but we 
                // could get fancier here.  For example we might pass 
                // in some constraint that requires the address be 
                // geocode -able with a certain precision (e.g., when 
                // a postal code centroid isn't good enough).

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
