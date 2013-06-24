using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebServer.Utils
{
    public static class RequestParser
    {

        static Regex _requestLineRegex = new Regex(@"(?<method>[^\s]+)\s+(?<uri>[^\s]+)\s+(?<authority>[^\s]+)");
        static Regex _headerLineRegex = new Regex(@"(?<key>[^:]+):(?<value>.+)");

        public static bool ParseRequestLine(string requestLine, out string method, out string uri, out string authority)
        {
            var match = _requestLineRegex.Match(requestLine);

            method = uri = authority = null;
            if (!match.Success) return false;

            method = match.Groups["method"].Value;
            uri = match.Groups["uri"].Value;
            authority = match.Groups["authority"].Value;

            return true;

        }

        public static bool ParseHeaderLine(string headerLine, out string key, out string value)
        {
            var match = _headerLineRegex.Match(headerLine);

            key = value = null;
            if (!match.Success) return false;

            key = match.Groups["key"].Value;
            value = match.Groups["value"].Value;

            return true;

        }
    }
}
