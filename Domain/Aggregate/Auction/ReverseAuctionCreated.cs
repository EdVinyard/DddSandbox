namespace Domain.Aggregate.Auction.Event
{
    public class ReverseAuctionCreated : InterAggregateEvent 
    {
        public int Id { get; }

        public ReverseAuctionCreated(ReverseAuction created)
        {
            Precondition.MustNotBeNull(created, nameof(created));
            Id = created.Id;
        }
    }
}
