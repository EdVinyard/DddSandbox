namespace Domain.Port
{
    /// <summary>
    /// In order to keep our Domain model "pure" (that is, free 
    /// from dependencies) this is interface will be satisfied at
    /// runtime by a thin facade over the StructureMap Nested
    /// Container specific to the request being handled.
    /// </summary>
    public interface IDependencies
    {
        T Instance<T>() where T : class;
    }
}
