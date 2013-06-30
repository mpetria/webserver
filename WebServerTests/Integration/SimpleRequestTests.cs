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
        [Test]
        public void SimpleGet()
        {
            var serverUri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
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
            var serverUri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
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
            var serverUri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
            var uri = serverUri + "/home2.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri);
            request.Method = "PUT";
            request.ContentType = "text/html";
            const string requestBody = @"<html><body>Hello PUT</body><html>";
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
        public void ChunkedPut()
        {
            var serverUri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
            var uri = serverUri + "/home2.html";

            Console.WriteLine(uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "text/html";
            request.SendChunked = true;

            
      
            string[] requestBodyChunks = { @"<html><body>First chunk", @"Second chunk</body><html>"};
            
            using (var requestStream = request.GetRequestStream())
            {
                for (int i = 0; i < requestBodyChunks.Length; i++)
                {
                    var requestBodyBytes = new ASCIIEncoding().GetBytes(requestBodyChunks[i]);
                    requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
                }
                    
            }

            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }


        [Test]
        public void ExpectPost()
        {
            var serverUri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
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
            var serverUri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
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
    }
}
