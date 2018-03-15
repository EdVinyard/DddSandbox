using Domain.Aggregate.Common;

namespace Domain.Aggregate.Bid
{
    // TODO: This verges on Smurf-naming, because I haven't figured
    // out how to name things when the aggregate name and the aggregate
    // root Entity name seem to be the same thing.
    public sealed class BidAggregate
    {
        public int Id => Root.Id;

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
        /// This is a Domain Service needed to *create* an Aggregate.
        /// It could optionally be exposed as a static method on the
        /// Aggregate itself that takes an explicit Port.IDependencies
        /// argument, like <c>AlterPickup()</c> does.  However, the
        /// name scoping and convention make it fairly easy to discover
        /// and use.
        /// </summary>
        public class Factory : Domain.Service
        {
            private readonly IInterAggregateEventBus _interAggregateEventBus;

            public Factory(IInterAggregateEventBus interAggregateEventBus)
            {
                _interAggregateEventBus = interAggregateEventBus;
            }

            public BidAggregate New(
                int reverseAuctionId,
                TimeRange pickupTime,
                TimeRange dropoffTime,
                Money price)
            {
                // RULE: All validation and precondition checking
                // is delegated down to the Factorys.  That keeps the
                // Aggregate code clean and simple.  

                // TODO: Ideally, exposing this at the aggregate level has the 
                // potential to allow all the Entity Factorys themselves to be 
                // hidden within this assembly, and never used outside it 
                // I haven't figured out how to do that, in practice.

                var bid = new Bid(
                    reverseAuctionId,
                    pickupTime,
                    dropoffTime,
                    price);

                // TODO: According to IDDD, we'd publish a "created" event
                // here.  I need to fit this into the context of our work
                // to wrap DB transactions around business interactions.
                // OTOH, there may be no conflict if the domain event 
                // publisher is flexible enough that in test the event is
                // published immediately but in production the event is
                // only published when the transaction is committed.  
                // However, if the consistency boundary IS the aggregate,
                // it may not be reasonable to wait.

                var aggregate = new BidAggregate(bid);

                _interAggregateEventBus.Publish(
                    new Event.BidCreated(aggregate));

                return aggregate;
            }
        }

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
