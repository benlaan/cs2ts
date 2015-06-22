using System;
using System.Collections.Generic;
using System.Linq;

namespace cs2ts
{
    public static class EnumerableExtensions
    {
        public static string ToCsv<T>(this IEnumerable<T> elements)
        {
            return String.Join(", ", elements.Select(e => e.ToString()));
        }
    }
}
