using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{
    public class Request
    {
        public Request()
        {
            Headers = new Dictionary<string, string>();
        }

        public string Method { get; set; }
        public string Host { get; set; }
        public string Uri { get; set; }

        public IDictionary<string, string> Headers { get; set; }


        public string GetHeaderValue(string key)
        {
            var lookupKey = key.ToLower();
            if (!Headers.ContainsKey(lookupKey))
                return null;
            return Headers[lookupKey];
        }
    }
}
