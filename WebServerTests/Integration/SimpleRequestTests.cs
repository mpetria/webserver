using System;
using System.Collections.Generic;
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
    }
}
