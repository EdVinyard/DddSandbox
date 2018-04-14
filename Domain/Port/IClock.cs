using System;

namespace Domain.Port
{
    public interface IClock : DDD.Port
    {
        DateTimeOffset Now { get; }
    }
}
