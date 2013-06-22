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

        public void AddReponseStatus()
        {
            ResponseString = @"HTTP/1.1 200 OK";
            ResponseString += "\nDate: Fri, 31 Dec 1999 23:59:59 GMT";
        }

        public void AddResponseHeader(string key, string value)
        {
            ResponseString += String.Format("\n{0}:{1}", key, value);
        }

        public void AddASCIIBody(string body)
        {
            AddResponseHeader("Content-Length", body.Length.ToString());
            ResponseString += String.Format("\n\n{0}", body);
        }

        private void CalculateBytes()
        {
            ResponseBytes = _asciiEncoding.GetBytes(ResponseString);
        }

        public static RawResponse BuildRawResponse(Response response)
        {
            var raw = new RawResponse();
            raw.AddReponseStatus();
            foreach (var pair in response.Headers)
            {
                raw.AddResponseHeader(pair.Key, pair.Value);
            }

            if (!String.IsNullOrEmpty(response.ContentType))
                raw.AddResponseHeader("Content-Type", response.ContentType);

            raw.AddASCIIBody(response.Body);
            raw.CalculateBytes();
            return raw;
        }
    }
}
