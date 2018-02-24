using System;

namespace Domain.Port
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }
}
