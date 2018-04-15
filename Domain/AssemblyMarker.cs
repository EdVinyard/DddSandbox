namespace Domain
{
    /// <summary>
    /// Used by Domain structural tests to identify the Domain assembly.
    /// </summary>
    public class AssemblyMarker
    {
        private AssemblyMarker()
        {
            throw new System.NotImplementedException(
                "This class is used only to locate the Domain assembly; " +
                "it must never be instantiated, but StructureMap does not " +
                "allow it to be static.");
        }
    }
}
