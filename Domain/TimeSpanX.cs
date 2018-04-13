using System;

namespace Domain
{
    /// <summary>
    /// This property belongs in a Base Class Library Extension project,
    /// as it's just a convenience method we wish were already included in
    /// the .NET BCL.
    /// </summary>
    public static class TimeSpanX
    {
        public static bool IsPositive(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks > 0;
        }
    }
}
