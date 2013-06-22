using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{
    public class RawResponse
    {
        public byte[] ResponseBytes { get; set; }
        public string ResponseString { get; set; }

        private ASCIIEncoding _asciiEncoding = new ASCIIEncoding();

        public void AddStatusCode(ResponseStatusCode statusCode)
        {
            var statusDescription = ResponseStatusDescription.Default[statusCode];
            ResponseString = String.Format("HTTP/1.1 {0} {1}\n", (int) statusCode, statusDescription);
        }

        public void AddDateHeader()
        {
            //RFC 1123 Date
            AddHeader("Date", DateTime.UtcNow.ToString("r"));
        }

        public void AddEmptyLine()
        {
            ResponseString += "\n";
        }

        public void AddHeader(string key, string value)
        {
            ResponseString += String.Format("{0}: {1}\n", key, value);
        }

        public void AddASCIIBody(string body)
        {
            AddHeader("Content-Length", body.Length.ToString());
            AddEmptyLine();
            ResponseString += String.Format("{0}", body);
        }

        private void CalculateBytes()
        {
            ResponseBytes = _asciiEncoding.GetBytes(ResponseString);
        }

        public static RawResponse BuildRawResponse(Response response)
        {
            var raw = new RawResponse();
            raw.AddStatusCode(response.StatusCode);
            raw.AddDateHeader();
            foreach (var pair in response.Headers)
            {
                raw.AddHeader(pair.Key, pair.Value);
            }

            if (!String.IsNullOrEmpty(response.ContentType))
                raw.AddHeader("Content-Type", response.ContentType);

            if(response.Body != null)
                raw.AddASCIIBody(response.Body);
            

            raw.CalculateBytes();
            return raw;
        }

        public static RawResponse BuildRawResponse(ResponseStatusCode statusCode)
        {
            return BuildRawResponse(new Response() {StatusCode = statusCode, Body = String.Empty});
        }
    }
}
