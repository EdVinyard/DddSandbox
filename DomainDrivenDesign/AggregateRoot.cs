namespace DDD
{
    public class AggregateRoot : Entity
    {
        public virtual int Version { get; protected set; }
    }
}
