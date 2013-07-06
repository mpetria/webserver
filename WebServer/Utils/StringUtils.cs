using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Utils
{
    public static class StringUtils
    {
        public static string SubstringBefore(this string str, char c)
        {
            int pos = str.IndexOf(c);
            if (pos != -1)
            {
                return str.Substring(0, pos);
            }
            return str;
        }
        public static string SubstringAfter(this string str, char c)
        {
            int pos = str.IndexOf(c);
            if (pos != -1)
            {
                if (pos == str.Length - 1) return String.Empty;
                return str.Substring(pos+1);
            }
            return str;
        }
    }
}
