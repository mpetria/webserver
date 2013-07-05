using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebServer.Utils;

namespace WebServer.Data
{
    public class RawRequest
    {
        public string RequestLine { get; set; }
        public IList<string> HeaderLines { get; set; }
        public byte[] Body { get; set; }
        public int? ContentLength { get; set; }
        public bool IsChunkedTransferEncoding { get; set; }
        public bool Expects100Continue { get; set; }
        public bool CloseConnection { get; set; }

        private readonly ASCIIEncoding _asciiEncoding = new ASCIIEncoding();


        public RawRequest()
        {
            HeaderLines = new List<string>();
        }

        public void AddHeaderLine(byte[] headerLineBytes)
        {
            string headerLine = _asciiEncoding.GetString(headerLineBytes);
            HeaderLines.Add(headerLine);

            string key, value;
            RequestParser.ParseHeaderLine(headerLine, out key, out value);

            if(key == HttpHeader.ContentLength)
            {
                int contentLength;
                int.TryParse(value, out contentLength);
                ContentLength = contentLength;
            }
            else if(key == HttpHeader.TransferEncoding && value == "chunked")
            {
                IsChunkedTransferEncoding = true;
            }
            else if (key == HttpHeader.Expect && value == "100-continue")
            {
                Expects100Continue = true;
            }
            else if (key == HttpHeader.Connection && value == "close")
            {
                CloseConnection = true;
            }
        }

        public void AddRequestLine(byte[] requestLineBytes)
        {
            RequestLine = _asciiEncoding.GetString(requestLineBytes);
        }

        public void AddBody(byte[] bodyBytes)
        {
            Body = bodyBytes;
        }

        public static Request BuildRequest(RawRequest rawRequest)
        {
            string method;
            string uri;
            string version;

            RequestParser.ParseRequestLine(rawRequest.RequestLine, out method, out uri, out version);

            var request = new Request() {Method = method, Version = version};


            foreach (var headerLine in rawRequest.HeaderLines)
            {
                string key, value;
                RequestParser.ParseHeaderLine(headerLine, out key, out value);
                request.AddHeader(key, value);
            }


            /*
               1. If Request-URI is an absoluteURI, the host is part of the Request-URI. Any Host header field value in the request MUST be ignored.
               2. If the Request-URI is not an absoluteURI, and the request includes a Host header field, the host is determined by the Host header field value.
               3. If the host as determined by rule 1 or 2 is not a valid host on the server, the response MUST be a 400 (Bad Request) error message.  
            */
            if(Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                var tempUri = new Uri(uri, UriKind.Absolute);
                request.PathAndQuery = tempUri.PathAndQuery;
                request.Host = tempUri.Host;
            }
            else if (Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                request.PathAndQuery = uri;
                request.Host = request.GetHeaderValue(HttpHeader.Host);
            }

            if (String.IsNullOrEmpty(request.PathAndQuery))
            {
                request.PathAndQuery = "/";
            }

            request.PathAndQuery = Uri.UnescapeDataString(request.PathAndQuery);

            request.Body = rawRequest.Body;
            return request;
        }
    }
}
