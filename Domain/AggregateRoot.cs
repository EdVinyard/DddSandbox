namespace Domain
{
    public class AggregateRoot : Entity
    {
        protected internal virtual int Version { get; set; }
    }
}
