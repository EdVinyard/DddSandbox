using Repr = Application.Representation;
using Domain.Aggregate.Auction;
using Domain.Aggregate.Common;
using Framework;
using System;

namespace Application
{
    // TODO: Is this the concrete form of a Resource?
    public static partial class ReverseAuction
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
                    Convert.FromRepr(repr.Pickup),
                    repr.Dropoff.Address,
                    Convert.FromRepr(repr.Dropoff),
                    repr.OtherTerms,
                    Convert.ToTimeRange(repr.BiddingStart, repr.BiddingEnd));
            }

            private Repr.ReverseAuction ToRepresentation(ReverseAuctionAggregate auction)
            {
                auction.MustNotBeNull(nameof(auction));

                return new Repr.ReverseAuction
                {
                    Uri = _uriScheme.ToUri(auction).ToString(),
                    BiddingStart = auction.BiddingAllowed.Start.ToIso8601(),
                    BiddingEnd = auction.BiddingAllowed.End.ToIso8601(),
                    Bids = Repr.PaginatedSequence<Repr.Bid>.Empty,
                    Pickup = Convert.ToRepr(auction.BuyerTerms.Pickup),
                    Dropoff = Convert.ToRepr(auction.BuyerTerms.Dropoff),
                    OtherTerms = auction.BuyerTerms.OtherTerms,
                };
            }
        }
    }
}
