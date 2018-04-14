namespace DDD
{
    /// <summary>
    /// <p>
    /// Each Port represents a reason the Domain (application) is trying 
    /// to talk with the outside world.  That is, a Service that is 
    /// implemented outside the Domain.  Examples include a clock, a 
    /// geocoding web service, and a database-backed Repository 
    /// implementation.
    /// </p>
    /// <p>
    /// For further reading, start at http://wiki.c2.com/?PortsAndAdaptersArchitecture
    /// </p>
    /// </summary>
    public interface Port : Service
    {
    }
}
