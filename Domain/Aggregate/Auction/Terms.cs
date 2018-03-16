using Domain.Port;
using System;
using System.Collections.Generic;

namespace Domain.Aggregate.Auction
{
    public class Terms : ValueType, IEquatable<Terms>
    {
        /// <summary>
        /// A stateless Domain Service that creates instances of Terms.
        /// <summary>
        public class Factory : Service
        {
            private readonly IClock _clock;

            public Factory(IClock clock) { _clock = clock;  }

            public Terms New(Waypoint pickup, Waypoint dropoff, string otherTerms)
            {
                Precondition.MustNotBeNull(pickup, nameof(pickup));
                Precondition.MustNotBeNull(dropoff, nameof(dropoff));

                if (pickup.Time.End <= _clock.Now)
                {
                    throw new ArgumentOutOfRangeException(
                        "pickup.Time.End must be in the future, " +
                        $"but it was {pickup.Time.End}", 
                        nameof(pickup));
                }

                if (dropoff.Time.End <= _clock.Now)
                {
                    throw new ArgumentOutOfRangeException(
                        "dropoff.Time.End must be in the future, " +
                        $"but it was {dropoff.Time.End}",
                        nameof(dropoff));
                }

                if (pickup.Time.End > dropoff.Time.End)
                {
                    throw new ArgumentException(
                        "pickup.Time.End must be earlier than dropoff.Time.End",
                        nameof(dropoff));
                }

                return new Terms
                {
                    Pickup = pickup,
                    Dropoff = dropoff,
                    OtherTerms = otherTerms ?? string.Empty,
                };
            }
        }

        /// <summary>
        /// FOR NHibernate and Terms.Factory ONLY!
        /// </summary>        
        protected Terms() { }

        public virtual Waypoint Pickup { get; protected set; }
        public virtual Waypoint Dropoff { get; protected set; }
        public virtual string OtherTerms { get; protected set; }

        public override bool Equals(object otherObj)
        {
            if (null == otherObj) return false;
            if (ReferenceEquals(this, otherObj)) return true;
            var otherTerms = otherObj as Terms;
            if (null == otherTerms) return false;
            return this.Equals(otherTerms);
        }

        public bool Equals(Terms other)
        {
            return (Pickup == other.Pickup)
                && (Dropoff == other.Dropoff)
                && (OtherTerms == other.OtherTerms);
        }

        public static bool operator ==(Terms l, Terms r) => Object.Equals(l, r);
        public static bool operator !=(Terms l, Terms r) => !(l == r);

        public override int GetHashCode()
        {
            var hashCode = 399892970;
            hashCode = hashCode * -1521134295 + EqualityComparer<Waypoint>.Default.GetHashCode(Pickup);
            hashCode = hashCode * -1521134295 + EqualityComparer<Waypoint>.Default.GetHashCode(Dropoff);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OtherTerms);
            return hashCode;
        }

        // TODO: How do we force client code, even within the Domain model,
        // to use ReverseAuctionAggregate.AlterPickupSvc() service instead 
        // of this method?
        internal void ChangePickup(Location pickup)
        {
            Pickup = new Waypoint(pickup, Pickup.Time);
        }
    }
}
