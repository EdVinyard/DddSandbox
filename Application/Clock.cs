using Domain.Port;
using System;

namespace Application
{
    public class Clock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
