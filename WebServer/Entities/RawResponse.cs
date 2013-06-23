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

        public void AddContentLengthAndBeginBody(int length)
        {
            AddHeader("Content-Length", length.ToString());
            AddEmptyLine();
        }

        private void CalculateBytes(byte[] additionalBytes)
        {
            additionalBytes = additionalBytes ?? new byte[0];
            ResponseBytes = _asciiEncoding.GetBytes(ResponseString).Concat(additionalBytes).ToArray();
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


            byte[] bodyBytes = null;
            if (response.Body != null)
            {
                bodyBytes = new ASCIIEncoding().GetBytes(response.Body);
            }
            else if (response.BodyBytes != null)
            {
                bodyBytes = response.BodyBytes;
            }

            if (bodyBytes != null)
            {
                raw.AddContentLengthAndBeginBody(bodyBytes.Length);
            }

            raw.CalculateBytes(bodyBytes);
            return raw;
        }

        public static RawResponse BuildRawResponse(ResponseStatusCode statusCode)
        {
            return BuildRawResponse(new Response() {StatusCode = statusCode, Body = String.Empty});
        }
    }
}
