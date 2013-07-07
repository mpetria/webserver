using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WebServerTests.Integration
{
    public class RequestHelper
    {

        private readonly string _serverUri;
        private readonly int _port;

        public RequestHelper(string serverUri, int port)
        {
            _serverUri = serverUri;
            _port = port;
        }

        private string GetResourceUri(string path)
        {
            var serverUri = String.Format("http://{0}:{1}", _serverUri, _port);
            var uri = serverUri.TrimEnd('/') + "/" + path.TrimStart('/');
            return uri;
        }

        private static HttpStatusCode ReadStatusCode(WebRequest request)
        {
            using (var response = ReadResponse(request))
            {
                return response.StatusCode;
            }
        }

        private static HttpWebResponse ReadResponse(WebRequest request)
        {
            
            try
            {
             
                return (HttpWebResponse)request.GetResponse();
                

            }
            catch (WebException we)
            {
                return (HttpWebResponse) we.Response;
            }
        }

        public HttpStatusCode Get(string path)
        {
            var uri = GetResourceUri(path);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";


            var responseStatus = ReadStatusCode(request);

            return responseStatus;

        }

        public HttpStatusCode Delete(string path)
        {
            var uri = GetResourceUri(path);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "DELETE";


            var responseStatus = ReadStatusCode(request);

            return responseStatus;

        }

        public HttpStatusCode Put(string path, byte[] requestBodyBytes)
        {
            var uri = GetResourceUri(path);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            var responseStatus = ReadStatusCode(request);

            return responseStatus;

        }

        public HttpStatusCode Put(string path, string htmlBody)
        {
            var uri = GetResourceUri(path);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "text/html";
            var requestBodyBytes = new ASCIIEncoding().GetBytes(htmlBody);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            var responseStatus = ReadStatusCode(request);

            return responseStatus;

        }

        public HttpStatusCode Head(string path)
        {
            var uri = GetResourceUri(path);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "HEAD";


            var responseStatus = ReadStatusCode(request);

            return responseStatus;

        }

        public HttpStatusCode Post(string path, string textBody, out string newResourceUri)
        {
            var uri = GetResourceUri(path);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/plain";
            var requestBodyBytes = new ASCIIEncoding().GetBytes(textBody);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            HttpStatusCode result;
            using(var response = ReadResponse(request))
            {
                newResourceUri = response.Headers[HttpResponseHeader.ContentLocation];
                result = response.StatusCode;
            }

            return result;

        }

    }
}
