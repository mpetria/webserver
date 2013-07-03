using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebServer.Data;

namespace WebServerTests.Unit
{
    [TestFixture]
    public class RawRequestTests
    {
        [Test]
        public void TestHostAndAbsPath()
        {
            var rawRequest = new RawRequest();
            rawRequest.RequestLine = @"GET /a.html HTTP/1.1";
            rawRequest.HeaderLines.Add("Host: www.test.com");
            var request = RawRequest.BuildRequest(rawRequest);

            Assert.AreEqual("www.test.com", request.Host);
        }

        [Test]
        public void TestHostAndAbsUri()
        {
            var rawRequest = new RawRequest();
            rawRequest.RequestLine = @"GET http://www.test.com/a.html HTTP/1.1";
            rawRequest.HeaderLines.Add("Host: www.example.com");
            var request = RawRequest.BuildRequest(rawRequest);

            Assert.AreEqual("www.test.com", request.Host);
        }
    }
}
