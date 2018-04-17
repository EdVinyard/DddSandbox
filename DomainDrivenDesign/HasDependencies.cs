namespace DDD
{
     /// <summary>
     /// Domain types either have state or have dependencies, but
     /// never both.  Types that have dependencies include Commands,
     /// Queries, Factories, and Repositories.
     /// </summary>
     public interface HasDependencies { }
}
