using DDD;
using Domain.Aggregate.Common;
using System;

namespace Domain.Aggregate.Bid
{
    // TODO: This verges on Smurf-naming, because I haven't figured
    // out how to name things when the aggregate name and the aggregate
    // root Entity name seem to be the same thing.  It may be easier to
    // avoid this name "collision" when the Aggregate is composed of
    // multiple Entities (but that's not a situation we should aspire to.)
    public sealed class BidAggregate : IAggregate
    {
        public int Id => Root.Id;

        public Money Price => Root.Price;
        public bool IsTendered => throw new NotImplementedException();
        public bool IsWithdrawn => throw new NotImplementedException();

        /// <summary>
        /// Withdraw a bid that was tendered.  The bidder is no longer
        /// willing to provide the services with these terms.
        /// This operation is idempotent, and does not throw if the
        /// bid has already been withdrawn.
        /// </summary>
        public void Withdraw()
        {
            throw new NotImplementedException();
            // 1. if the bid was already cancelled, return
            // 2. cancel the bid; save it
            // 3. publish BidWithdrawnEvent
        }

        /// <summary>
        /// Exposed for ReverseAuctionRepository use only.
        /// (I haven't figured out how to hide this more
        /// effectively.  Should I rename it "_Root" as a
        /// warning?)
        /// </summary>
        internal Bid Root { get; private set; }

        /// <summary>
        /// Exposed for testing and debugging only.
        /// </summary>
        internal int Version => Root.Version;

        /// <summary>
        /// Exposed for ReverseAuctionRepository use only.
        /// (I haven't figured out how to hide this more
        /// effectively.  Should I expose a only as an internal 
        /// static method, "_New()" as a warning?)
        /// </summary>
        internal BidAggregate(Bid bid)
        {
            this.Root = bid;
        }
    }
}
