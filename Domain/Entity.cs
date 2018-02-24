using System;

namespace Domain
{
    public abstract class Entity : IEquatable<Entity>
    {
        public virtual int Id { get; protected set; }

        /// <summary>
        /// When you create an Entity instance, until you first
        /// save it, this returns true; thereafter it returns false.
        /// </summary>
        protected bool IsTransient => 0 == Id;

        public override bool Equals(object otherObj)
        {
            if (null == otherObj) return false;
            if (ReferenceEquals(this, otherObj)) return true;
            var otherEntity = otherObj as Entity;
            if (null == otherEntity) return false;
            return this.Equals(otherEntity);
        }

        public virtual bool Equals(Entity other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;
            if (IsTransient ^ other.IsTransient) return false;
            if (IsTransient && other.IsTransient) return ReferenceEquals(this, other);
            return this.Id == other.Id;
        }

        private int? _cachedHashCode;
        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue)
            {
                _cachedHashCode = IsTransient 
                    ? base.GetHashCode() 
                    : Id.GetHashCode();
            }

            return _cachedHashCode.Value;
        }

        public static bool operator==(Entity l, Entity r) => Object.Equals(l, r);
        public static bool operator!=(Entity l, Entity r) => !(l == r);
    }
}
