using Domain.Aggregate.Common;

namespace Domain.Aggregate.Bid
{
    public class Bid : AggregateRoot
    {
        private int _reverseAuctionId;

        /// <summary>
        /// Aggregates refer to other Aggregates by identifier only,
        /// not by containment or direct reference.
        /// </summary>
        public virtual int ReverseAuctionId
        {
            get { return _reverseAuctionId; }
            protected set { _reverseAuctionId = value; }
        }

        private TimeRange _pickupTime;
        public virtual TimeRange PickupTime
        {
            get { return _pickupTime; }
            protected set { _pickupTime = value; }
        }

        private TimeRange _dropoffTime;
        public virtual TimeRange DropoffTime
        {
            get { return _dropoffTime; }
            protected set { _dropoffTime = value; }
        }

        private Money _price;
        public virtual Money Price
        {
            get { return _price; }
            protected set { _price = value; }
        }

        /// <summary>
        /// For use by ReverseAuction.PlaceBid() factory method ONLY!
        /// TODO: How can this be hidden better, even from other types 
        /// in the Domain?
        /// </summary>
        internal Bid(
            int reverseAuctionId,
            TimeRange pickupTime,
            TimeRange dropoffTime,
            Money price)
        {
            Precondition.MustNotBeNull(pickupTime, nameof(pickupTime));
            Precondition.MustNotBeNull(dropoffTime, nameof(dropoffTime));
            Precondition.MustNotBeNull(price, nameof(price));

            _reverseAuctionId = reverseAuctionId;
            _pickupTime = pickupTime;
            _dropoffTime = dropoffTime;
            _price = price;
        }

        /// <summary>
        /// FOR NHibernate ONLY!
        /// </summary>        
        protected Bid() { }
    }
}
