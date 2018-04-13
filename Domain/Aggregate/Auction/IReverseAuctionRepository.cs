using DDD;
using System;
using System.Collections.Generic;

namespace Domain.Aggregate.Auction
{
    // TODO: This Repository interface doesn't conform to either
    // of the recommended designs, in particular, the Update() method.
    //
    // Collection-Oriented: the "traditional DDD approach because it adheres to 
    // the basic ideas presented in the original DDD pattern. These very 
    // closely mimic a collection, simulating at least some of its standard 
    // interface. Here you design a Repository interface that does not hint 
    // in any way that there is an underlying persistence mechanism, avoiding 
    // any notion of saving or persisting data to a store." 
    //                              - IDDD, Vernor, Chapter: Repositories
    //
    // Persistence-Oriented: you "must explicitly put() both new and changed 
    // objects into the store, effectively replacing any value previously 
    // associated with the given key. Using these kinds of data stores 
    // greatly simplifies the basic writes and reads of Aggregates."
    //                              - IDDD, Vernor, Chapter: Repositories
    //
    // These offer the advantage that they are more amenable to changes in
    // persistence mechanism, for example from a relational database to a
    // Object- or Document-oriented data store.

    public interface IReverseAuctionRepository : IRepository
    {
        // TODO: To preserve the illusion of an in-memory Set as the 
        // Repository, should this method be named "Add"?
        ReverseAuctionAggregate Save(ReverseAuctionAggregate ra);

        // TODO: In a Collection-Oriented repository, mimicking a Set, this
        // could simply be the indexer-getter (i.e., `repository[id]`), but
        // this is OK as is.
        ReverseAuctionAggregate Get(int id);

        // TODO: Is this method needed?  If NHibernate is tracking dirty 
        // (changed) instances, and the FlushMode is set correctly, I think
        // this method is unneccessary.
        ReverseAuctionAggregate Update(ReverseAuctionAggregate ra);

        // TODO: Convert the following method into an example of a "use-case
        // optimal query" that cuts across Aggregate boundaries and returns
        // a collection of Value Types.  In this case, return a Value Type 
        // "projection" that cuts across the ReverseAuction and Bid Aggregates, 
        // including information that would normally be accessible only by 
        // navigation from each root, separately.  A direct query is
        // optimization, but it's OK!  We'll NEVER use the returned Value
        // Types to mutate the Domain directly, but often the end-user will 
        // direct some mutation based on the identifiers returned in the 
        // Value Type projection.

        // "This is where you specify a complex query against the persistence 
        // mechanism, dynamically placing the results into a Value Object(6) 
        // specifically designed to address the needs of the use case.
        //
        // It should not seem strange for a Repository to in some cases answer 
        // a Value Object rather than an Aggregate instance.A Repository that 
        // provides a size() method answers a very simple Value in the form 
        // of an integer count of the total Aggregate instances it holds.  A 
        // use case optimal query is just extending this notion a bit to 
        // provide a somewhat more complex Value, one that addresses more 
        // complex client demands."
        //
        //                              - IDDD, Vernor, Chapter: Repositories

        /// <summary>
        /// Find all auctions that are "live" (i.e., currently accepting bids)
        /// and return them one page at a time.
        /// </summary>
        IReadOnlyList<ReverseAuction> GetLive(
            DateTimeOffset dt,
            int pageSize,
            int pageIndex);

        // "If you find that you must create many finder methods supporting 
        // use case optimal queries on multiple Repositories, it’s probably 
        // a code smell.First of all, this situation could be an indication 
        // that you’ve misjudged Aggregate boundaries and overlooked the 
        // opportunity to design one or more Aggregates of different types.
        // The code smell here might be called Repository masks Aggregate 
        // mis-design.
        // 
        // However, what if you encounter this situation and your analysis 
        // indicates that your Aggregate boundaries are well designed? This 
        // could point to the need to consider using CQRS (4)."
        //
        //                              - IDDD, Vernor, Chapter: Repositories
    }
}
