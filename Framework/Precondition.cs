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
    }
}
