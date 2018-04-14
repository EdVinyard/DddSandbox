using DDD;
using Domain.Aggregate.Common;
using Domain.Port;
using Framework;
using System;

namespace Domain.Aggregate.Auction
{
    // TODO: This verges on Smurf-naming, because I haven't figured
    // out how to name things when the aggregate name and the aggregate
    // root Entity name seem to be the same thing.
    public sealed class ReverseAuctionAggregate : IAggregate
    {
        // Notice that these pass-through properties are getter-only.
        // We'd prefer to expose mutators only in ways that makes it 
        // extremely clear when there are *any* side-effects or 
        // dependencies.

        public int Id => Root.Id;
        public Terms BuyerTerms => Root.BuyerTerms;
        public TimeRange BiddingAllowed => Root.BiddingAllowed;

        /// <summary>
        /// Exposed for ReverseAuctionRepository use only.
        /// (I haven't figured out how to hide this more
        /// effectively.  Should I rename it "_Root" as a
        /// warning?)
        /// </summary>
        internal ReverseAuction Root { get; private set; }

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
        public class Factory : DDD.Factory
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

        /// <summary>
        /// This is a Domain Service needed to mutate an Aggregate.
        /// In order to avoid both the Service Location Pattern and
        /// dependencies in our Aggregate, we separate it out, thus.
        /// This has nice qualities:
        /// 
        /// 1) It looks like a separate CQS type, but client code
        ///    doesn't invoke it directly.  Additionally, CQS *classes*
        ///    can make the operations available on an Aggregate more
        ///    difficult to discover (which means they're more likely
        ///    to be re-implemented in a slightly different way).
        ///    
        /// 2) Dependencies for a single Aggregate method are clear
        ///    and hopefully concise which eases testing, and makes 
        ///    the methods cheaper.
        /// </summary>
        private class _AlterPickup : ICommand
        {
            // RULE: Domain Services classes nested inside an IAggregate
            // must contain a single constructor and a single public
            // method.  They should be either a Command or a Query, not 
            // both.

            private Location.Factory _locationFactory;
            private IInterAggregateEventBus _interAggregateEventBus;

            public _AlterPickup(
                Location.Factory locationFactory,
                IInterAggregateEventBus interAggregateEventBus)
            {
                _locationFactory = locationFactory;
                _interAggregateEventBus = interAggregateEventBus;
            }

            public void AlterPickup(
                ReverseAuctionAggregate aggregate, 
                string pickupAddress)
            {
                var pickup = _locationFactory.New(pickupAddress);
                aggregate.Root.BuyerTerms.ChangePickup(pickup);

                _interAggregateEventBus.Publish(new Event.BuyerTermsChanged(aggregate));
            }
        }

        /// <summary>
        /// Alter the pickup location.
        /// 
        /// Aggregate methods that have dependencies are implemented as 
        /// "hidden" (private, nested) Service classes (AlterPickupSvc in 
        /// this case) and exposed to clients on the Aggregate interface
        /// just like this.  This method *must* be a pass-through only, with
        /// no logic whatsoever.
        /// 
        /// We get as much of the best of both words as we can:
        /// - Client authors see a concise argument list.
        /// - Test authors see dependencies enumerated on the hidden 
        ///   service's constructor.
        /// </p>
        /// </summary>
        /// <param name="di">
        /// A dependency injector.  Using a thin facade atop what'll be 
        /// StructureMap at runtime keeps our Domain model pure (entirely 
        /// lacking in external dependencies) and means that switching DI
        /// Containers later doesn't modify the Domain at all.
        /// </param>
        /// <param name="newPickupAddress">
        /// </param>
        public void AlterPickup(Port.IDependencies di, string newPickupAddress)
        {
            di.Instance<_AlterPickup>()
                .AlterPickup(this, newPickupAddress);
        }

        /// <summary>
        /// This Domain Service creates a *different* Aggregate, 
        /// a Bid.  It must be the *only* way a Bid is ever created.
        /// </summary>
        private class _PlaceBid : ICommand
        {
            private readonly IInterAggregateEventBus _interAggregateEventBus;
            private readonly IClock _clock;
            private readonly Bid.Bid.Factory _bidFactory;

            public _PlaceBid(
                IInterAggregateEventBus interAggregateEventBus,
                IClock clock,
                Bid.Bid.Factory bidFactory)
            {
                _interAggregateEventBus = interAggregateEventBus;
                _clock = clock;
                _bidFactory = bidFactory;
            }

            public Bid.BidAggregate PlaceBid(
                ReverseAuctionAggregate reverseAuction,
                TimeRange pickupTime,
                TimeRange dropoffTime,
                Money price)
            {
                Precondition.MustNotBeNull(reverseAuction, nameof(reverseAuction));

                // RULE: We'd like to delegate validation and precondition 
                // checking to Factorys.  That keeps the Aggregate code 
                // clean and simple...
                // but we need to enforce a relationship between the Auction
                // and the new Bid.

                BiddingMustBeAllowedNow(reverseAuction);
                BidMustConformToAuctionTime(
                    reverseAuction.Root.BuyerTerms.Pickup,
                    nameof(pickupTime),
                    pickupTime);
                BidMustConformToAuctionTime(
                    reverseAuction.Root.BuyerTerms.Dropoff,
                    nameof(dropoffTime),
                    dropoffTime);
                PriceMustBeNonNegative(price);

                var bid = _bidFactory.New(
                    reverseAuction.Id,
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

                var aggregate = new Bid.BidAggregate(bid);

                _interAggregateEventBus.Publish(
                    new Bid.Event.BidCreated(aggregate));

                return aggregate;
            }

            private void BiddingMustBeAllowedNow(ReverseAuctionAggregate reverseAuction)
            {
                Precondition.MustNotBeNull(reverseAuction, nameof(reverseAuction));

                if (!reverseAuction.BiddingAllowed.Includes(_clock.Now))
                {
                    throw new BiddingNotAllowedNow(reverseAuction, _clock);
                }
            }

            private void PriceMustBeNonNegative(Money price)
            {
                Precondition.MustNotBeNull(price, nameof(price));

                if (price.IsNegative)
                {
                    throw new NegativePriceProhibited(price);
                }
            }

            private void BidMustConformToAuctionTime(
                Waypoint auctionWaypoint,
                string argumentName,
                TimeRange bidTime)
            {
                Precondition.MustNotBeNull(auctionWaypoint, nameof(auctionWaypoint));
                Precondition.MustNotBeNull(argumentName, nameof(argumentName));
                Precondition.MustNotBeNull(bidTime, nameof(bidTime));

                if (!auctionWaypoint.Time.Includes(bidTime))
                {
                    throw new BidTimeDisagreesWithAuctionTime(
                        auctionWaypoint,
                        argumentName,
                        bidTime);
                }
            }
        }

        [Serializable]
        public class BiddingNotAllowedNow : InvalidOperationException
        {
            internal BiddingNotAllowedNow(
                ReverseAuctionAggregate reverseAuction,
                IClock clock)
                : base(
                    $"at {clock.Now}, bidding is not allowed on " +
                    $"ReverseAuction {reverseAuction.Id}")
            { }
        }

        [Serializable]
        public class NegativePriceProhibited : ArgumentException
        {
            internal NegativePriceProhibited(Money price) : base(
                "the Bid price must not be negative; it was {price}")
            { }
        }

        [Serializable]
        public class BidTimeDisagreesWithAuctionTime : ArgumentException
        {
            internal BidTimeDisagreesWithAuctionTime(
                Waypoint auctionWaypoint,
                string paramName,
                TimeRange bidTime)
                : base(
                    $"the proposed Bid {paramName}, {bidTime}, is not " +
                    $"compatible with the Auction's {auctionWaypoint.Time}",
                    paramName)
            { }
        }

        public Bid.BidAggregate PlaceBid(
            Port.IDependencies di,
            TimeRange pickupTime,
            TimeRange dropoffTime,
            Money price)
        {
            return di.Instance<_PlaceBid>().PlaceBid(
                this,
                pickupTime, 
                dropoffTime, price);
        }

        /// <summary>
        /// Exposed for ReverseAuctionRepository use only.
        /// (I haven't figured out how to hide this more
        /// effectively.  Should I expose a only as an internal 
        /// static method, "_New()" as a warning?)
        /// </summary>
        internal ReverseAuctionAggregate(ReverseAuction auction)
        {
            this.Root = auction;
        }
    }
}
