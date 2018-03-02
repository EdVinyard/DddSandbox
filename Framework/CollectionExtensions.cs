using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Efficiently obtain an IReadOnlyList instance given an IList.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// when <c>list</c> is <c>null</c>
        /// </exception>
        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return list as IReadOnlyList<T>
                ?? new ReadOnlyList<T>(list);
        }

        private sealed class ReadOnlyList<T> : IReadOnlyList<T>
        {
            private readonly IList<T> list;
            public ReadOnlyList(IList<T> list) { this.list = list; }
            public int Count => list.Count;
            public T this[int index] => list[index];
            IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
            public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        }
    }
}
