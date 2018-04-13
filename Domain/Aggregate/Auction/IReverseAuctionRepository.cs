using DDD;
using System;
using System.Collections.Generic;

namespace Domain.Aggregate.Auction
{
    public interface IReverseAuctionRepository : IRepository
    {
        ReverseAuctionAggregate Save(ReverseAuctionAggregate ra);
        ReverseAuctionAggregate Get(int id);
        ReverseAuctionAggregate Update(ReverseAuctionAggregate ra);
        IReadOnlyList<ReverseAuction> GetLive(
            DateTimeOffset dt,
            int pageSize,
            int pageIndex);
    }
}
