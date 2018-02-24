using Domain.Port;
using System;
using System.Collections.Generic;

namespace Domain.Aggregate.Auction
{
    /// <summary>
    /// An auction in which one or more sellers vie to supply a service
    /// to a buyer.
    /// </summary>
    public class ReverseAuction : Entity
    {
        // RULE: Entities must always be constructed using a stateless 
        // "constructor service".  They should expose exactly one, 
        // protected, zero-argument constructor.  This guarantees:
        // 
        // - no compiler warnings about virtual method calls in constructor
        // 
        // - infrastructure in place to add external dependencies to 
        //   construction process later

        /// <summary>
        /// A stateless Domain Service that creates instances of
        /// ReverseAuction.
        /// <summary>
        public class Constructor : Service
        {
            private readonly IClock _clock;

            public Constructor(IClock clock) {
                _clock = clock;
            }

            public ReverseAuction New(Terms buyerTerms, TimeRange biddingAllowed)
            {
                Precondition.MustNotBeNull(buyerTerms, nameof(buyerTerms));
                Precondition.MustNotBeNull(biddingAllowed, nameof(biddingAllowed));

                if (biddingAllowed.End < _clock.Now)
                {
                    throw new ArgumentOutOfRangeException(
                        "biddingAllowed TimeRange must end in the future, " +
                        $"but it was {biddingAllowed}");
                }

                // PERK: Using a "constructor service" eliminates the compiler 
                // complaining about virtual method calls in the constructor.
                return new ReverseAuction
                {
                    BuyerTerms = buyerTerms,
                    BiddingAllowed = biddingAllowed,
                };
            }
        }

        /// <summary>
        /// FOR NHibernate and ReverseAuction.Constructor ONLY!
        /// </summary>        
        protected ReverseAuction() { }

        public virtual Terms BuyerTerms { get; protected set; }

        /// <summary>
        /// TODO: Based on the concurrency requirements described by the
        /// Implementing DDD book Aggregate chapter, I'm not sure if it's
        /// appropriate to lump these in here; they may need to be separate
        /// and referenced only by identifier.
        /// <summary>
        //public virtual IReadOnlyList<Bid> Bids { get; protected set; }

        public virtual TimeRange BiddingAllowed { get; protected set; }
    }
}
