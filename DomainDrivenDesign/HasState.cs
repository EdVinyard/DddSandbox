namespace DDD
{
    /// <summary>
    /// Domain types either have state or have dependencies, but
    /// never both.  Stateful types include ValueTypes, Entities,
    /// and Aggregates.  You should not implement this interface
    /// directly.  Rather, you should implement one of the marker
    /// interfaces that implement this.
    /// </summary>
    public interface HasState
    {
    }
}
