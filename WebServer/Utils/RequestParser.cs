using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebServer.Utils
{
    public static class RequestParser
    {

        static Regex _requestLineRegex = new Regex(@"(?<method>[^\s]+)\s+(?<uri>[^\s]+)\s+(?<version>[^\s]+)");
        static Regex _headerLineRegex = new Regex(@"(?<key>[^:]+):(?<value>.+)");

        public static bool ParseRequestLine(string requestLine, out string method, out string uri, out string version)
        {
            var match = _requestLineRegex.Match(requestLine);

            method = uri = version = null;
            if (!match.Success) return false;

            method = match.Groups["method"].Value;
            uri = match.Groups["uri"].Value;
            version = match.Groups["version"].Value;

            return true;

        }

        public static bool ParseHeaderLine(string headerLine, out string key, out string value)
        {
            var match = _headerLineRegex.Match(headerLine);

            key = value = null;
            if (!match.Success) return false;

            key = match.Groups["key"].Value.Trim();
            value = match.Groups["value"].Value.Trim();

            return true;

        }
    }
}
