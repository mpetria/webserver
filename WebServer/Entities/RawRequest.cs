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
        public int? ContentLength { get; set; }
        public bool IsChunkedTransferEncoding { get; set; }
        public bool Expects100Continue { get; set; }

        private ASCIIEncoding _asciiEncoding = new ASCIIEncoding();

        public void AddHeaderLine(byte[] headerLineBytes)
        {
            if(HeaderLines == null) 
                HeaderLines = new List<string>();

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
            string authority;

            RequestParser.ParseRequestLine(rawRequest.RequestLine, out method, out uri, out authority);
            

            var request = new Request() {Method = method, Uri = uri};


            foreach (var headerLine in rawRequest.HeaderLines)
            {
                string key, value;
                RequestParser.ParseHeaderLine(headerLine, out key, out value);
                request.AddHeader(key, value);
            }

            request.Body = rawRequest.Body;
            return request;
        }
    }
}
