using Domain.Port;
using System;

namespace ApplicationTest
{
    public class FakeClock : IClock
    {
        public FakeClock()
        {
            Now = new DateTimeOffset(
                2018, 2, 3,
                4, 13, 59,
                TimeSpan.FromHours(1));
        }

        public DateTimeOffset Now { get; set; }
    }
}
