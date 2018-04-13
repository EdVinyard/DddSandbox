namespace DDD
{
    /// <summary>
    /// <p>
    /// A Command is a type of Domain Service that mutates a Value Type, 
    /// Entity, or Aggregate, and has Dependencies.  It exposes only a 
    /// constructor (for dependency injection) and a single public
    /// method that (typically) returns null.  It may also return
    /// a value that indicates the success (true/false) or 
    /// magnitude of the change performed (number of Foos Bazzled).
    /// </p>
    /// <p>
    /// A Command is nested inside an Aggregate, Entity, or Value Type
    /// and exposed via a method on that Type's public interface.  An
    /// example follows.
    /// </p>
    /// <code>
    /// 
    /// public class Bank : IAggregate {
    /// 
    ///     public void IssueCreditCard(
    ///         IDependencies deps, 
    ///         CCApplication app) 
    ///     {
    ///         deps.Instance<_IssueCreditCard>()
    ///             .IssueCreditCard(this, app);
    ///     }
    ///     
    ///     internal class _IssueCreditCard : ICommand 
    ///     {
    ///         private readonly ICreditChecker _checker;
    ///         public _IssueCreditCard(ICreditChecker checker) {
    ///             _checker = checker;
    ///         }
    ///         public void IssueCreditCard(Bank bank, CCApplication app) {
    ///             checker.ThrowUnlessCreditWorthy(appForm);
    ///             bank._creditCardAccounts.Add( ... );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </summary>
    public interface ICommand : Service
    {
    }
}
