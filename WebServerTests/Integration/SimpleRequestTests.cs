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
            var uri = String.Format("http://{0}:{1}", ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
            Console.WriteLine(uri+"/home.html");

            var request = WebRequest.Create(uri);
            request.Method = "PUT";

            using(var response = request.GetResponse())
            {
                Console.WriteLine(response.ToString());
            }
        }
    }
}
