using Domain.Aggregate.Auction;
using Framework;
using NHibernate;
using System.Collections.Generic;

namespace Persistence
{
    public class ReverseAuctionRepository : IReverseAuctionRepository
    {
        private readonly ISession session;

        public ReverseAuctionRepository(ISession session)
        {
            this.session = session;
        }

        public ReverseAuction Save(ReverseAuction ra)
        {
            return (ReverseAuction)this.session.Save(ra);
        }

        public ReverseAuction Update(ReverseAuction ra)
        {
            this.session.Update(ra);
            return ra;
        }

        public ReverseAuction Get(int id)
        {
            return this.session.Get<ReverseAuction>(id);
        }

        public IReadOnlyList<ReverseAuction> GetRecent(
            int pageSize, 
            int pageIndex)
        {
            return this.session
                .QueryOver<ReverseAuction>()
                .OrderBy(ra => ra.Id)
                .Desc
                .Take(pageSize)
                .Skip((pageIndex - 1) * pageSize)
                .List()
                .AsReadOnly();
        }
    }
}
