using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Utils;

namespace WebServer.Entities
{
    public class RawResponse
    {
        public byte[] ResponseBytes { get; set; }
        public string ResponseString { get; set; }
        public Stream ResponseStream { get; set; }

        private ASCIIEncoding _asciiEncoding = new ASCIIEncoding();

        public void AddStatusCode(ResponseStatusCode statusCode)
        {
            var statusDescription = ResponseStatusDescription.DefaultDescriptions[statusCode];
            ResponseString = String.Format("HTTP/1.1 {0} {1}", (int) statusCode, statusDescription);
            AddEndLine();
        }

        public void AddDateHeader(string key, DateTime dateTime)
        {
            var dateString = DateUtils.GetFormatedServerDate(dateTime);
            AddHeader(key, dateString);
        }
      

        public void AddEndLine()
        {
            ResponseString += "\r\n";
        }

        public void AddHeader(string key, string value)
        {
            ResponseString += String.Format("{0}: {1}", key, value);
            AddEndLine();
        }

        public void AddContentLengthAndBeginBody(int length)
        {
            AddHeader(HttpHeader.ContentLength, length.ToString());
            AddEndLine();
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
            
            
            raw.AddDateHeader(HttpHeader.Date, DateTime.Now);
            foreach (var pair in response.Headers)
            {
                raw.AddHeader(pair.Key, pair.Value);
            }

            if (!String.IsNullOrEmpty(response.ContentType))
                raw.AddHeader(HttpHeader.ContentType, response.ContentType);

            if (response.LastModified != null)
            {
                //raw.AddHeader("Cache-Control", "public, max-age=0");
                raw.AddDateHeader(HttpHeader.LastModified, response.LastModified.Value);
            }


            byte[] bodyBytes = null;
            if (response.Body != null)
            {
                bodyBytes = new ASCIIEncoding().GetBytes(response.Body);
            }
            else if (response.BodyBytes != null)
            {
                bodyBytes = response.BodyBytes;
            }

            if(response.StatusCode == ResponseStatusCode.Continue || response.StatusCode == ResponseStatusCode.NoContent || response.StatusCode == ResponseStatusCode.NotModified)
            {
                if(bodyBytes != null)
                    throw new Exception("Body not allowed for this response code");
            }
            else if (response.SuppressBody)
            {
                bodyBytes = null;
            }
            else if (bodyBytes == null &&  response.BodyStream != null)
            {
                bodyBytes = new byte[0];
            }
           

            if (bodyBytes != null)
            {
                raw.AddContentLengthAndBeginBody(bodyBytes.Length);
            }
            else
            {
                raw.AddEndLine();
            }

            raw.ResponseStream = response.BodyStream;

            raw.CalculateBytes(bodyBytes);
            return raw;
        }

        public static RawResponse BuildRawResponse(ResponseStatusCode statusCode)
        {
            return BuildRawResponse(new Response() {StatusCode = statusCode});
        }
    }
}
