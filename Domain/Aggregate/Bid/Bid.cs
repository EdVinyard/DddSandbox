using DDD;
using Domain.Aggregate.Common;
using Domain.Port;
using Framework;
using System;

namespace Domain.Aggregate.Bid
{
    public class Bid : AggregateRoot
    {
        /// <summary>
        /// Aggregates refer to other Aggregates by identifier only,
        /// not by containment or direct reference.
        /// </summary>
        public virtual int ReverseAuctionId { get; protected set; }
        public virtual TimeRange PickupTime { get; protected set; }
        public virtual TimeRange DropoffTime { get; protected set; }
        public virtual Money Price { get; protected set; }

        public virtual bool IsTendered => !WithdrawalDate.HasValue;
        public virtual bool IsWithdrawn => !IsTendered;
        public virtual DateTimeOffset? WithdrawalDate { get; protected set; }

        public class Factory : DDD.Factory
        {
            public Factory() { }

            public Bid New(
                int reverseAuctionId,
                TimeRange pickupTime,
                TimeRange dropoffTime,
                Money price)
            {
                Precondition.MustNotBeNull(pickupTime, nameof(pickupTime));
                Precondition.MustNotBeNull(dropoffTime, nameof(dropoffTime));
                Precondition.MustNotBeNull(price, nameof(price));

                return new Bid
                {
                    ReverseAuctionId = reverseAuctionId,
                    PickupTime = pickupTime,
                    DropoffTime = dropoffTime,
                    Price = price,
                };
            }
        }

        /// <summary>
        /// FOR NHibernate AND Bid.Factory ONLY!
        /// </summary>        
        protected Bid() { }

        /// <summary>
        /// for Aggregate use only
        /// </summary>
        protected internal virtual void WithdrawNow(IDependencies deps)
        {
            if (IsTendered)
            {
                deps.Instance<_WithdrawNow>()
                    .WithdrawNow(this);
            }
        }

        private class _WithdrawNow : ICommand
        {
            private readonly IClock _clock;
            public _WithdrawNow(IClock c) { _clock = c; }
            public void WithdrawNow(Bid b) { b.WithdrawalDate = _clock.Now; }
        }
    }
}
