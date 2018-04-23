using System;
using Domain.Aggregate.Auction;
using Domain.Port;
using Framework;
using Repr = Application.Representation;

namespace Application
{
    public static partial class ReverseAuction
    {
        public class AlterPickup : ApplicationService
        {
            private readonly IReverseAuctionRepository _repository;
            private readonly IUriScheme _uriScheme;
            private readonly IDependencies _dependencies;

            public AlterPickup(
                IReverseAuctionRepository repository,
                IUriScheme uriScheme,
                IDependencies dependencies)
            {
                _repository = repository;
                _uriScheme = uriScheme;
                _dependencies = dependencies;
            }

            public Repr.Waypoint To(
                int reverseAuctionId,
                Repr.Waypoint newPickup)
            {
                newPickup.MustNotBeNull(nameof(newPickup));

                // This assertion knows too much about underlying details.
                // It seems like another clue that we need a Domain Value Type
                // that represents a ReverseAuctionIdentifier.
                //reverseAuctionId.MustBePositive(nameof(reverseAuctionId));

                var auction = _repository.Get(reverseAuctionId);

                // TODO: There's a substantial, intentional mismatch bettween
                // the coarser API Waypoint resource and the finer-grained
                // Domain Waypoint, Location, and TimeRange concepts.  Right
                // now, only changes to the location are allowed.  Pickup
                // TimeRange changes should be allowed, but require both more
                // business logic and some thought about how the two fit 
                // together.
                ThrowIfPickupTimeRangeHasChanged(auction, newPickup);

                auction.AlterPickup(
                    _dependencies,
                    newPickup.Address);

                _repository.Update(auction);
                return Convert.ToRepr(auction.BuyerTerms.Pickup);
            }

            /// <summary>EXPOSED FOR UNIT TESTING ONLY</summary>
            internal void ThrowIfPickupTimeRangeHasChanged(
                ReverseAuctionAggregate auction, 
                Repr.Waypoint newPickup)
            {
                var current = auction.BuyerTerms.Pickup.Time;
                var requested = Convert.ToTimeRange(newPickup.Earliest, newPickup.Latest);

                if (current != requested)
                {
                    throw new ArgumentException("cannot change pickup time");
                }
            }
        }
    }
}
