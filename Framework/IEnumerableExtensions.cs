using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns true iff the sequence contains no elements.  If you
        /// have to write a null check first, consider <c>IsNullOrEmpty()</c>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// when <c>ts</c> is null
        /// </exception>
        public static bool IsEmpty<T>(this IEnumerable<T> ts) => !ts.Any();

        /// <summary>
        /// Returns true iff <c>ts</c> is either null or contains no elements.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// when <c>ts</c> is null
        /// </exception>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> ts)
        {
            return (null == ts) || !ts.Any();
        }
    }
}
