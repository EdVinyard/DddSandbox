namespace DDD
{
    /// <summary>
    /// A Factory is a Domain Service that creates/manufactures instances of
    /// a Value Type, Entity, or Aggregate.  If a Factory
    /// exists for one of those types, it MUST be the ONLY way to create an
    /// instance of that Type.
    /// <summary>
    public interface Factory : Service
    {
    }
}
