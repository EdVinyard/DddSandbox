using Domain.Aggregate;
using Domain.Aggregate.Auction;
using FluentNHibernate.Mapping;

namespace Persistence
{
    internal sealed class NHibernateMappingMarker { }

    internal class ReverseAuctionMap : ClassMap<ReverseAuction>
    {
        public ReverseAuctionMap()
        {
            Id(x => x.Id);
            Component(x => x.BuyerTerms).ColumnPrefix("BuyerTerms_");
            Component(x => x.BiddingAllowed).ColumnPrefix("BiddingAllowed_");
            //HasMany(x => x.Bids); // Not if Bids belong in a separate aggregate!
        }
    }

    internal class TermsMap : ComponentMap<Terms>
    {
        public TermsMap()
        {
            Component(x => x.Pickup).ColumnPrefix("Pickup_");
            Component(x => x.Dropoff).ColumnPrefix("Dropoff_");
            Map(x => x.OtherTerms);
        }
    }

    internal class WaypointMap : ComponentMap<Waypoint>
    {
        public WaypointMap()
        {
            Component(x => x.Time).ColumnPrefix("Time_");
            Component(x => x.Place).ColumnPrefix("Place_");
        }
    }

    internal class TimeRangeMap : ComponentMap<TimeRange>
    {
        public TimeRangeMap()
        {
            Map(x => x.Start);
            Map(x => x.Duration).Column("DurationSec");
        }
    }

    internal class BidMap : ClassMap<Bid>
    {
        public BidMap()
        {
            Id(x => x.Id);
            Component(x => x.SellerTerms).ColumnPrefix("SellerTerms_");
            Component(x => x.Price).ColumnPrefix("Price_");
        }
    }

    internal class MoneyMap : ComponentMap<Money>
    {
        public MoneyMap()
        {
            Map(x => x.Amount);
            Map(x => x.CurrencyCode);
        }
    }

    internal class LocationMap : ComponentMap<Location>
    {
        public LocationMap()
        {
            Map(x => x.Address);
            Component(l => l.Coordinates, m =>
            {
                m.Map(c => c.Latitude);
                m.Map(c => c.Longitude);
            });
        }
    }
}
