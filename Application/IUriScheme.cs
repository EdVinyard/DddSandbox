using Domain.Aggregate.Auction;
using System;

namespace Application
{
    public interface IUriScheme // TODO a true Application Service
    {
        Uri ToUri(ReverseAuctionAggregate reverseAuction);
    }
}
