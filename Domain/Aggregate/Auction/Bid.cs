namespace Domain.Aggregate.Auction
{
    public class Bid : Entity
    {
        public virtual Terms SellerTerms { get; protected set; }
        public virtual Money Price { get; protected set; }
    }
}
