namespace Domain.Port
{
    /// <summary>
    /// <p>
    /// In order to keep our Domain model "pure" (that is, free 
    /// from dependencies) this is interface will be satisfied at
    /// runtime by a thin facade over the StructureMap Nested
    /// Container specific to the request being handled.
    /// </p>
    /// <p>
    /// Because the call-stack is the ultimate scope of the 
    /// dependency, neither a global, nor static, nor singleton
    /// way to obtain an instance of this type must ever exist!
    /// </p>
    /// </summary>
    public interface IDependencies
    {
        T Instance<T>() where T : class;
    }
}
