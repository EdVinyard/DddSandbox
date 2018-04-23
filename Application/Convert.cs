using Domain.Aggregate.Auction;
using Domain.Aggregate.Common;
using Framework;
using System;
using Repr = Application.Representation;

namespace Application
{
    /// <summary>
    /// Methods that convert between Domain types and Representations 
    /// (serializable DTOs).  A more sophisticated implementation might
    /// factor this out into one or many Assembler classes (see 
    /// https://martinfowler.com/eaaCatalog/dataTransferObject.html) to
    /// increase Application Service test-ability.
    /// </summary>
    internal static class Convert
    {
        internal static TimeRange FromRepr(this Repr.Waypoint w)
        {
            w.MustNotBeNull(nameof(w));

            return ToTimeRange(w.Earliest, w.Latest);
        }

        internal static TimeRange ToTimeRange(string startIso8601, string endIso8601)
        {
            startIso8601.MustNotBeNull(nameof(startIso8601));
            endIso8601.MustNotBeNull(nameof(endIso8601));

            // TODO: Translate from Domain-oriented errors to UI-oriented errors.

            // TODO: Push this all down into a new TimeRange constructor.
            var start = DateTimeOffset.Parse(startIso8601);
            var end = DateTimeOffset.Parse(endIso8601);
            var duration = end - start;
            return new TimeRange(start, duration);
        }

        internal static Repr.Waypoint ToRepr(this Waypoint w) => new Repr.Waypoint
        {
            Address = w.Place.Address,
            Latitude = w.Place.Coordinates.Latitude,
            Longitude = w.Place.Coordinates.Longitude,
            Earliest = w.Time.Start.ToIso8601(),
            Latest = w.Time.End.ToIso8601(),
        };
    }
}
