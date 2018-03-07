namespace Domain.Aggregate.Auction.Event
{
    public class ReverseAuctionEvent : InterAggregateEvent
    {
        public int Id { get; }

        /// <summary>
        /// DomainEvents have <c>internal</c> constructors because they should
        /// only be constructed within the Domain project.  Nothing outside 
        /// the Domain should ever publish or create one.
        /// </summary>
        internal ReverseAuctionEvent(ReverseAuctionAggregate created)
        {
            Precondition.MustNotBeNull(created, nameof(created));
            Id = created.Id;
        }
    }

    public class ReverseAuctionCreated : ReverseAuctionEvent
    {
        internal ReverseAuctionCreated(ReverseAuctionAggregate x) : base(x) { }
    }

    public class BuyerTermsChanged : ReverseAuctionEvent
    {
        internal BuyerTermsChanged(ReverseAuctionAggregate x) : base(x) { }
    }
}
