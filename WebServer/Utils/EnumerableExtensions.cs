using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Utils
{
    public static class EnumerableExtensions
    {
        public static string JoinWithSeparator<T>(this IEnumerable<T> target, string separator)
        {
            return target.Aggregate(String.Empty, (temp, x) => String.Format("{0}{1}{2}", temp, String.IsNullOrEmpty(temp)? String.Empty: separator, x.ToString()));
        }
    }
}
