namespace DDD
{
    /// <summary>
    /// A Port is a Service that is implemented outside the Domain.
    /// Examples include a clock, an outside web service, and a Repository.
    /// </summary>
    public interface Port : Service
    {
    }
}
