namespace Domain
{
    /// <summary>
    /// a marker interface to clearly distinguish between Entity and Value 
    /// types in the Domain

    // RULE: Any value type that is used by more than one
    // Aggregate should be "promoted" to some common area to make
    // it clear that it is, e.g., a generic TimeRange rather than
    // specifically a ReverseAuction.TimeRange.  In general Value
    // Types should be more generically constructed than Entities,
    // because they're lower-level concepts and more likely to be
    // broadly applicable or useful.  The degenerate example of this
    // are the bulit-in types: int, string, etc. that'll be used
    // everywhere, but also DateTimeOffset, TimeSpan, other such
    // general concepts.
    //
    // Value Types may be shared by Aggregates within a Bounded 
    // Context.  Value Types may NOT be shared between Bounded 
    // Contexts, because even if they initially appear to have
    // the same meaning within those separate Contexts, it is
    // likely that their meaning will evolve or be discovered over
    // time to differ slightly.  The temptation to "reuse" those
    // Value Types must never be greater than the need to have
    // correct, appropriate, focused Types defined within every
    // Bounded Context, without "extra" features or functionality 
    // only appropriate in some other Context.
    /// <summary>
    public interface ValueType
    {
    }
}
