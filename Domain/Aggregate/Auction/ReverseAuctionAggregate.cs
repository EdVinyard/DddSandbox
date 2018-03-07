namespace Domain.Aggregate.Auction
{
    // TODO: This verges on Smurf-naming, because I haven't figured
    // out how to name things when the aggregate name and the aggregate
    // root Entity name seem to be the same thing.
    public sealed class ReverseAuctionAggregate
    {
        internal ReverseAuction Root { get; private set; }
        public int Id => Root.Id;
        internal int Version => Root.Version; // TODO: Remove this?
        public Terms BuyerTerms => Root.BuyerTerms;
        public TimeRange BiddingAllowed => Root.BiddingAllowed;

        public class Factory : Domain.Service
        {
            private Location.Factory _locationFactory;
            private ReverseAuction.Factory _reverseAuctionFactory;
            private Terms.Factory _termsFactory;
            private IInterAggregateEventBus _interAggregateEventBus;

            public Factory(
                Location.Factory locationFactory,
                ReverseAuction.Factory reverseAuctionFactory,
                Terms.Factory termsFactory,
                IInterAggregateEventBus interAggregateEventBus)
            {
                _locationFactory = locationFactory;
                _reverseAuctionFactory = reverseAuctionFactory;
                _termsFactory = termsFactory;
                _interAggregateEventBus = interAggregateEventBus;
            }

            public ReverseAuctionAggregate New(
                string pickupAddress,
                TimeRange pickupTime,
                string dropoffAddress,
                TimeRange dropoffTime,
                string otherTerms,
                TimeRange biddingAllowed)
            {
                // RULE: All validation and precondition checking
                // is delegated down to the Factorys.  That keeps the
                // Aggregate code clean and simple.  

                // TODO: Ideally, exposing this at the aggregate level has the 
                // potential to allow all the Entity Factorys themselves to be 
                // hidden within this assembly, and never used outside it 
                // I haven't figured out how to do that, in practice.

                var pickup = new Waypoint(
                    _locationFactory.New(pickupAddress),
                    pickupTime);
                var dropoff = new Waypoint(
                    _locationFactory.New(dropoffAddress),
                    dropoffTime);
                var auction = _reverseAuctionFactory.New(
                    _termsFactory.New(pickup, dropoff, otherTerms),
                    biddingAllowed);

                // TODO: According to IDDD, we'd publish a "created" event
                // here.  I need to fit this into the context of our work
                // to wrap DB transactions around business interactions.
                // OTOH, there may be no conflict if the domain event 
                // publisher is flexible enough that in test the event is
                // published immediately but in production the event is
                // only published when the transaction is committed.  
                // However, if the consistency boundary IS the aggregate,
                // it may not be reasonable to wait.

                var aggregate = new ReverseAuctionAggregate(auction);

                _interAggregateEventBus.Publish(
                    new Event.ReverseAuctionCreated(aggregate));

                return aggregate;
            }
        }

        public class AlterPickupLocation : Service
        {
            private Location.Factory _locationFactory;
            private IInterAggregateEventBus _interAggregateEventBus;

            public AlterPickupLocation(
                Location.Factory locationFactory,
                IInterAggregateEventBus interAggregateEventBus)
            {
                _locationFactory = locationFactory;
                _interAggregateEventBus = interAggregateEventBus;
            }

            public ReverseAuctionAggregate ChangePickup(
                ReverseAuctionAggregate aggregate, 
                string pickupAddress)
            {
                var pickup = _locationFactory.New(pickupAddress);
                aggregate.Root.BuyerTerms.ChangePickup(pickup);

                _interAggregateEventBus.Publish(new Event.BuyerTermsChanged(aggregate));

                return aggregate;
            }
        }

        /// <summary>
        /// exposed as internal only for the Repository; it would be
        /// better were it just private
        /// </summary>
        internal ReverseAuctionAggregate(ReverseAuction auction)
        {
            this.Root = auction;
        }
    }
}
