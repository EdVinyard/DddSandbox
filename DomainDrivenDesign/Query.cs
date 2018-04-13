namespace DDD
{
    /// <summary>
    /// <p>
    /// A Query is a type of Domain Service that retrieves information from 
    /// a Value Type, Entity, or Aggregate, and has Dependencies.  It exposes 
    /// only a constructor (for dependency injection) and a single public
    /// method that (typically) returns the requested information.  It does
    /// not mutate the state of the Type on which it is defined.
    /// </p>
    /// <p>
    /// A Query is nested inside an Aggregate, Entity, or Value Type
    /// and exposed via a method on that Type's public interface.  An
    /// example follows.
    /// </p>
    /// <code>
    /// 
    /// public class Bank : IAggregate {
    /// 
    ///     public void DeliquentAccounts(IDependencies deps) 
    ///     {
    ///         deps.Instance<_DeliquentAccounts>()
    ///             .IssueCreditCard(this);
    ///     }
    ///     
    ///     internal class _DeliquentAccounts : ICommand 
    ///     {
    ///         private readonly IClock _clock;
    ///         public _DeliquentAccounts(IClock clock) {
    ///             _clock = clock;
    ///         }
    ///         public IEnumerable<Account> DeliquentAccounts(Bank bank) {
    ///             return _creditCardAccounts
    ///                 .Where(acct => acct.DueDate < _clock.Now);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </summary>
    public interface IQuery : Service
    {
    }
}
