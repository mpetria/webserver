using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebServer.Utils;

namespace WebServerTests.Unit
{
    [TestFixture]
    public class DateUtilsTests
    {
        [Test]
        public void TestLegacyFormats()
        {
            string[] dates = {
                                 "Sun, 06 Nov 1994 08:49:37 GMT", //  ; RFC 822, updated by RFC 1123
                                 "Sunday, 06-Nov-94 08:49:37 GMT", // ; RFC 850, obsoleted by RFC 1036
                                 "Sun Nov 6 08:49:37 1994" //  ; ANSI C's asctime() format}
                             };


            var d1 = DateUtils.ParseClientDate(dates[0]);
            var d2 = DateUtils.ParseClientDate(dates[1]);
            var d3 = DateUtils.ParseClientDate(dates[2]);

            Assert.AreEqual(d1, d2);
            Assert.AreEqual(d2, d3);

        }

        [Test]
        public void TestEqualityWithLegacyFormats()
        {
            string[] dates = {
                                 "Sun, 06 Nov 1994 08:49:37 GMT", //  ; RFC 822, updated by RFC 1123
                                 "Sunday, 06-Nov-94 08:49:37 GMT", // ; RFC 850, obsoleted by RFC 1036
                                 "Sun Nov 6 08:49:37 1994" //  ; ANSI C's asctime() format}
                             };


            foreach (var date in dates)
            {
                var isMatch = DateUtils.CheckIfHttpDatesMatch("Sun, 06 Nov 1994 08:49:37 GMT", date);
                Assert.IsTrue(isMatch);
                
            }
        }
    }
}
