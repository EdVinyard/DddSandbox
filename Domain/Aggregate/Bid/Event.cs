namespace Domain.Aggregate.Bid.Event
{
    public class BidEvent : InterAggregateEvent
    {
        public int Id { get; }

        /// <summary>
        /// DomainEvents have <c>internal</c> constructors because they should
        /// only be constructed within the Domain project.  Nothing outside 
        /// the Domain should ever publish or create one.
        /// </summary>
        internal BidEvent(BidAggregate created)
        {
            Precondition.MustNotBeNull(created, nameof(created));
            Id = created.Id;
        }
    }

    public class BidCreated : BidEvent
    {
        internal BidCreated(BidAggregate x) : base(x) { }
    }
}
