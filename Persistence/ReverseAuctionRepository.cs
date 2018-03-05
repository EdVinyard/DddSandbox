using Domain.Aggregate.Auction;
using Framework;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Persistence
{
    public class ReverseAuctionRepository : IReverseAuctionRepository
    {
        private readonly ISession session;

        public ReverseAuctionRepository(ISession session)
        {
            this.session = session;
        }

        public ReverseAuctionAggregate Save(ReverseAuctionAggregate ra)
        {
            this.session.Save(ra.Root);
            return ra;
        }

        public ReverseAuctionAggregate Update(ReverseAuctionAggregate ra)
        {
            this.session.Update(ra.Root);
            return ra;
        }

        public ReverseAuctionAggregate Get(int id)
        {
            return new ReverseAuctionAggregate(
                this.session.Get<ReverseAuction>(id));
        }

        /// <summary>
        /// Returns all ReverseAuctions open for Bidding at the specified time.
        /// </summary>
        public IReadOnlyList<ReverseAuction> GetLive(
            DateTimeOffset dt,
            int pageSize, 
            int pageIndex)
        {
            return this.session
                .QueryOver<ReverseAuction>()
                .Where(ra =>
                    // TODO: prove this is equivalent to TimeRange.Includes()
                    // or factor out this logic (e.g., a common Expression).
                    (ra.BiddingAllowed.Duration == TimeSpan.Zero
                    && ra.BiddingAllowed.Start == dt) 
                    ||
                    (ra.BiddingAllowed.Start <= dt
                    && dt < ra.BiddingAllowed.End))
                .OrderBy(ra => ra.Id)
                .Desc
                .Take(pageSize)
                .Skip((pageIndex - 1) * pageSize)
                .List()
                .AsReadOnly();
        }
    }
}
