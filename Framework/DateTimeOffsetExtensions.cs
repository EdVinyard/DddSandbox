using System;

namespace Framework
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToIso8601(this DateTimeOffset dt)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
            return dt.ToString("O");
        }
    }
}
