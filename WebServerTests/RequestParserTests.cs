using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebServer.Utils;

namespace WebServerTests
{
    [TestFixture]
    public class RequestParserTests
    {

        [Test]
        public void RequestLineTest()
        {
            string method;
            string uri;
            string authority;
            RequestParser.ParseRequestLine("GET /a.html HTTP/1.1", out method, out uri, out authority);

            Assert.AreEqual("GET", method);
            Assert.AreEqual("/a.html", uri);
            Assert.AreEqual("HTTP/1.1", authority);
        }
    }
}
