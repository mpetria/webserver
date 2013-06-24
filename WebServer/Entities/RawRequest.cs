using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebServer.Utils;

namespace WebServer.Entities
{
    public class RawRequest
    {
        public string RequestLine { get; set; }
        public IList<string> HeaderLines { get; set; }
        public byte[] Body { get; set; }
        public int ContentLength { get; set; }
        public string BodyAsciiString { get; set; }

        private ASCIIEncoding _asciiEncoding = new ASCIIEncoding();

        public void AddHeaderLine(byte[] headerLineBytes)
        {
            if(HeaderLines == null) 
                HeaderLines = new List<string>();

            string headerLine = _asciiEncoding.GetString(headerLineBytes);
            HeaderLines.Add(headerLine);
            if(headerLine.StartsWith("Content-Length:"))
            {
                string contentLengthString = headerLine.Substring("Content-Length:".Length);
                int contentLength;
                int.TryParse(contentLengthString, out contentLength);
                ContentLength = contentLength;
            }
        }

        public void AddRequestLine(byte[] requestLineBytes)
        {
            RequestLine = _asciiEncoding.GetString(requestLineBytes);
        }

        public void AddBody(byte[] bodyBytes)
        {
            Body = bodyBytes;
            BodyAsciiString = _asciiEncoding.GetString(Body);
        }

        public static Request BuildRequest(RawRequest rawRequest)
        {
            string method;
            string uri;
            string authority;

            RequestParser.ParseRequestLine(rawRequest.RequestLine, out method, out uri, out authority);

            

            var request = new Request() {Method = method, Host = "", Uri = uri};


            foreach (var headerLine in rawRequest.HeaderLines)
            {
                string key, value;
                RequestParser.ParseHeaderLine(headerLine, out key, out value);
                request.AddHeader(key, value);
            }
            return request;
        }
    }
}
