﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Data
{
    public class Request
    {
        public string Method { get; set; }
        public string Host { get; set; }
        public string Version { get; set; }
        public string PathAndQuery { get; set; }
        public byte[] Body { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }

        public Request()
        {
            Headers = new Dictionary<string, string>();
        }

        public void AddHeader(string key, string value)
        {
            key = key.Trim().ToLower();
            value = value.Trim();

            Headers.Add(key, value);
        }

        public string GetHeaderValue(string key)
        {
            var lookupKey = key.ToLower();
            if (!Headers.ContainsKey(lookupKey))
                return null;
            return Headers[lookupKey];
        }

        public bool IsValid()
        {
            if (Version == HttpVersion.HTTP_1_1 && Host == null)
                return false;

            if (Method != HttpMethod.TRACE 
                && Method != HttpMethod.PUT 
                && Method != HttpMethod.POST 
                && Method != HttpMethod.OPTIONS 
                && Method != HttpMethod.HEAD 
                && Method != HttpMethod.GET 
                && Method != HttpMethod.DELETE 
                && Method != HttpMethod.CONNECT)
                return false;

            return true;

        }
    }
}
