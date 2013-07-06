using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using WebServer.Config;

namespace WebServerTests.Integration
{
    [TestFixture]
    public class SimpleRequestTests
    {

        private IPAddress _ipAddress = IPAddress.Loopback;
        private int _port = 9000;

        [Test]
        public void SimpleGet()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/home.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri);
            request.Method = "GET";

            using(var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }

        [Test]
        public void SimpleHead()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/home.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri);
            request.Method = "HEAD";

            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }

        [Test]
        public void SimplePut()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/home2.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "text/html";
            request.Headers.Add(HttpRequestHeader.ContentEncoding, "identity");
            
            const string requestBody = @"<html><body>Hello PUT2</body><html>";
            var requestBodyBytes = new ASCIIEncoding().GetBytes(requestBody);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }

        [Test]
        public void SimplePutWithUnimplementedEncoding()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/home2.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "text/html";
            request.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");

            const string requestBody = @"<html><body>Hello PUT2</body><html>";
            var requestBodyBytes = new ASCIIEncoding().GetBytes(requestBody);

            
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
                }


            var responseStatus = ReadStatusCode(request);
            Assert.AreEqual(HttpStatusCode.NotImplemented, responseStatus);
        }


        [Test]
        public void ChunkedPut()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/home2.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "text/html";
            request.SendChunked = true;
            request.Headers.Add(HttpRequestHeader.Trailer, "Connection");

            
      
            string[] requestBodyChunks = { @"<html><body>", @"First chunk", @"Second chunk", @"</body><html>"};
            
            using (var requestStream = request.GetRequestStream())
            {
                for (int i = 0; i < requestBodyChunks.Length; i++)
                {
                    var requestBodyBytes = new ASCIIEncoding().GetBytes(requestBodyChunks[i]);
                    requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
                }
                    
            }


            var responseStatus = ReadStatusCode(request);
            Assert.AreEqual(HttpStatusCode.OK, responseStatus);
        }


        [Test]
        public void ExpectPost()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/success.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/html";
            request.ServicePoint.Expect100Continue = true;



            const string requestBody = @"<html><body>Hello POST</body><html>";
            var requestBodyBytes = new ASCIIEncoding().GetBytes(requestBody);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }


        [Test]
        public void ExpectPut()
        {
            var serverUri = String.Format("http://{0}:{1}", _ipAddress, _port);
            var uri = serverUri + "/success.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "text/html";
            request.ServicePoint.Expect100Continue = true;



            const string requestBody = @"<html><body>Hello POST</body><html>";
            var requestBodyBytes = new ASCIIEncoding().GetBytes(requestBody);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }


        private HttpStatusCode ReadStatusCode(WebRequest request)
        {
            HttpStatusCode result;
            try
            {
                using(var wResp = (HttpWebResponse)request.GetResponse())
                {
                    result = wResp.StatusCode;
                }
                
            }
            catch (WebException we)
            {
                result = ((HttpWebResponse)we.Response).StatusCode;
            }
            return result;
        }
    }
}
