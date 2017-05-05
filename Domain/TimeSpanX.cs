using System;

namespace Domain
{
    public static class TimeSpanX
    {
        public static bool IsPositive(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks > 0;
        }
    }
}
