using System;

namespace UserInterface.Models
{
    // TODO: Do these DTO definitions belong in the Application layer,
    // because we'd expect them to be shared by different UIs?
    [Serializable]
    public class Representation
    {
    }

    // Intentionally create mismatches between the shape of these DTOs
    // and the shape of the Domain types.

    public class ReverseAuction : Representation
    {
        public string Uri;
        public Waypoint Pickup;
        public Waypoint Dropoff;
        public string OtherTerms;

        /// <summary>
        /// The first moment in time at which bidding is allowed, expressed as
        /// an ISO-8601 date-time with a specific offset.
        /// </summary>
        public string BiddingStart;

        /// <summary>
        /// The first moment in time at which bidding is no longer allowed, 
        /// expressed as an ISO-8601 date-time with a specific offset.
        /// </summary>
        public string BiddingEnd;

        /// <summary>
        /// all of the bids tendered
        /// </summary>
        public PaginatedSequence<Bid> Bids;
    }

    public class Waypoint : Representation
    {
        /// <summary>
        /// The street or physical address of the location, as a human being
        /// would understand it.  
        /// E.g., "1600 Pennsylvania Ave NW, Washington, DC 20500"
        /// </summary>
        public string Address;

        /// <summary>
        /// The latitude of the address, in the range [-90.0, 90.0]
        /// (i.e., including both -90 and 90).
        /// </summary>
        public double Latitude;

        /// <summary>
        /// The longitude of the address, in the range [-180.0, 180.0)
        /// (i.e., including -180 but excluding 180).
        /// </summary>
        public double Longitude;

        /// <summary>
        /// The first moment in time at which pickup may occur, expressed as
        /// an ISO-8601 date-time with a specific offset.
        /// </summary>
        public string Earliest;

        /// <summary>
        /// The first moment in time at which pickup may no longer occur, 
        /// expressed as an ISO-8601 date-time with a specific offset.
        /// </summary>
        public string Latest;
    }

    [Serializable]
    public class PaginatedSequence<TItem> where TItem : Representation
    {
        /// <summary>
        /// the total number of items available
        /// </summary>
        public int TotalCount;

        /// <summary>
        /// the maximum number of items included in a single page
        /// </summary>
        public int PageSize;

        /// <summary>
        /// the zero-based index of the current page
        /// </summary>
        public int PageIndex;

        /// <summary>
        /// the items on this page
        /// </summary>
        public TItem[] Page;

        /// <summary>
        /// the URI of the next page
        /// </summary>
        public string NextPageUri;
    }

    public class Bid : Representation
    {
        /// <summary>
        /// The first moment in time at which pickup may occur, expressed as
        /// an ISO-8601 date-time with a specific offset.
        /// </summary>
        public string Earliest;

        /// <summary>
        /// The first moment in time at which pickup may no longer occur, 
        /// expressed as an ISO-8601 date-time with a specific offset.
        /// </summary>
        public string Latest;

        /// <summary>
        /// The number of units of Currency for which the bidder will perform
        /// the job.
        /// </summary>
        public double Amount;

        /// <summary>
        /// The three character ISO-4217 code denoting the currency in which
        /// the Amount property is denominated.
        /// </summary>
        public string Currency;
    }
}
