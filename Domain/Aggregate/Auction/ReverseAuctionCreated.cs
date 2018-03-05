namespace Domain.Aggregate.Auction.Event
{
    public class ReverseAuctionCreated : InterAggregateEvent 
    {
        public int Id { get; }

        /// <summary>
        /// DomainEvents have <c>internal</c> constructors because they should
        /// only be constructed within the Domain project.  Nothing outside 
        /// the Domain should ever publish or create one.
        /// </summary>
        internal ReverseAuctionCreated(ReverseAuctionAggregate created)
        {
            Precondition.MustNotBeNull(created, nameof(created));
            Id = created.Id;
        }
    }
}
