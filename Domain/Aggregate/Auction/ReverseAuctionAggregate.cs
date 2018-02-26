namespace Domain.Aggregate.Auction
{
    // TODO: This verges on Smurf-naming, because I haven't figured
    // out how to name things when the aggregate name and the aggregate
    // root Entity name seem to be the same thing.
    public class ReverseAuctionAggregate
    {
        private Location.Factory _locationFactory;
        private ReverseAuction.Factory _reverseAuctionFactory;
        private Terms.Factory _termsFactory;
        private InterAggregateEventBus _interAggregateEventBus;

        public ReverseAuctionAggregate(
            Location.Factory locationFactory,
            ReverseAuction.Factory reverseAuctionFactory,
            Terms.Factory termsFactory,
            InterAggregateEventBus interAggregateEventBus)
        {
            _locationFactory = locationFactory;
            _reverseAuctionFactory = reverseAuctionFactory;
            _termsFactory = termsFactory;
            _interAggregateEventBus = interAggregateEventBus;
        }

        public ReverseAuction Create(
            string pickupAddress,
            TimeRange pickupTime,
            string dropoffAddress,
            TimeRange dropoffTime,
            string otherTerms,
            TimeRange biddingAllowed)
        {
            // RULE: All validation and precondition checking
            // is delegated down to the Factorys.  That keeps the
            // Aggregate code clean and simple.  Additionally, exposing this
            // method on the aggregate has the important effect of
            // allowing all the Factorys themselves to be hidden
            // within this aggregate, and never used outside it (although
            // I haven't figured out how to do that, in practice).

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
            _interAggregateEventBus.Publish(new Event.ReverseAuctionCreated(auction));

            return auction;
        }
    }
}
