using Repr = Application.Representation;
using Domain.Aggregate.Auction;
using Domain.Aggregate.Common;
using Framework;
using System;

namespace Application
{
    // TODO: Is this the concrete form of a Resource?
    public static class ReverseAuction
    {
        public class Create : ApplicationService
        {
            private readonly ReverseAuctionAggregate.Factory _factory;
            private readonly IReverseAuctionRepository _repository;
            private readonly IUriScheme _uriScheme;

            public Create(
                ReverseAuctionAggregate.Factory factory,
                IReverseAuctionRepository repository,
                IUriScheme uriScheme)
            {
                _factory = factory;
                _repository = repository;
                _uriScheme = uriScheme;
            }

            // TODO: Write a ReverseAuctionId ValueType instead of using an int?
            public Repr.ReverseAuction From(Repr.ReverseAuction repr)
            {
                return ToRepresentation(_repository.Save(ToAggregate(repr)));
            }

            // TODO: Move the following methods into separate Assembler classes.

            private ReverseAuctionAggregate ToAggregate(
                Repr.ReverseAuction repr)
            {
                repr.MustNotBeNull(nameof(repr));

                // TODO: Translate from Domain-oriented errors to UI-oriented errors.
                return _factory.New(
                    repr.Pickup.Address,
                    ToTimeRange(repr.Pickup),
                    repr.Dropoff.Address,
                    ToTimeRange(repr.Dropoff),
                    repr.OtherTerms,
                    ToTimeRange(repr.BiddingStart, repr.BiddingEnd));
            }

            private Repr.ReverseAuction ToRepresentation(
                ReverseAuctionAggregate auction)
            {
                auction.MustNotBeNull(nameof(auction));

                return new Repr.ReverseAuction
                {
                    Uri = _uriScheme.ToUri(auction).ToString(),
                    BiddingStart = auction.BiddingAllowed.Start.ToIso8601(),
                    BiddingEnd = auction.BiddingAllowed.End.ToIso8601(),
                    Bids = Repr.PaginatedSequence<Repr.Bid>.Empty,
                    Pickup = ToRepr(auction.BuyerTerms.Pickup),
                    Dropoff = ToRepr(auction.BuyerTerms.Dropoff),
                    OtherTerms = auction.BuyerTerms.OtherTerms,
                };
            }

            private static TimeRange ToTimeRange(Repr.Waypoint w)
            {
                w.MustNotBeNull(nameof(w));

                return ToTimeRange(w.Earliest, w.Latest);
            }

            private static TimeRange ToTimeRange(string startIso8601, string endIso8601)
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

            private static Repr.Waypoint ToRepr(Waypoint w) => new Repr.Waypoint
            {
                Address   = w.Place.Address,
                Latitude  = w.Place.Coordinates.Latitude,
                Longitude = w.Place.Coordinates.Longitude,
                Earliest  = w.Time.Start.ToIso8601(),
                Latest    = w.Time.End.ToIso8601(),
            };
        }
    }
}
