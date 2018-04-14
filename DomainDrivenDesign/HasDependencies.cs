namespace DDD
{
    /// <summary>
    /// Domain types either have state or have dependencies, but
    /// never both.  Types that have dependencies include Commands,
    /// Queries, Factories, and Repositories.  You should not 
    /// implement this interface directly.  Rather, you should 
    /// implement one of the marker interfaces that implement this.
    /// </summary>
    public interface HasDependencies
    {
    }
}
