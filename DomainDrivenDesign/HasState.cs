namespace DDD
{
     /// <summary>
     /// Domain types either have state or have dependencies, but
     /// never both.  Stateful types include ValueTypes, Entities,
     /// and Aggregates.
     /// </summary>
    public interface HasState { }
}
