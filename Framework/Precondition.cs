using System;

namespace Framework
{
    public static class Precondition
    {
        public static void MustNotBeNull(this object o, string paramName)
        {
            if (null != o) return;

            if (string.IsNullOrWhiteSpace(paramName))
            {
                paramName = "<unknown parameter name>";
            }

            throw new ArgumentNullException(paramName);
        }

        public static void MustBePositive(this int i, string paramName)
        {
            if (i > 0) return;

            if (string.IsNullOrWhiteSpace(paramName))
            {
                paramName = "<unknown parameter name>";
            }

            throw new ArgumentOutOfRangeException(paramName,
                $"parameter {paramName} must be a positive integer, " +
                $"but it was {i}");
        }
    }
}
