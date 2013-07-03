using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WebServer.Utils
{
    public class DateUtils
    {
        public static string GetFormatedHttpDateFromUtcDate(DateTime dateTime)
        {
            //RFC 1123 Date
            return dateTime.ToString("r");
        }

        public static string NormalizeToFormatedHttpDate(string clientHeaderTime)
        {
            var utcDate = ParseClientDate(clientHeaderTime);
            return utcDate == null ? null : utcDate.Value.ToString("r");
        }

        public static DateTime? ParseClientDate(string clientHeaderTime)
        {

            DateTime result;
            var formats = new[] {"r", // Sun, 06 Nov 1994 08:49:37 GMT ; RFC 822, updated by RFC 1123
                "dddd, dd-MMM-yy hh:mm:ss GMT", // Sunday, 06-Nov-94 08:49:37 GMT ; RFC 850, obsoleted by RFC 1036
                "ddd MMM d hh:mm:ss yyyy" // Sun Nov 6 08:49:37 1994  ; ANSI C's asctime() format
            };

            if (DateTime.TryParseExact(clientHeaderTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out result))
                return result;

            return null;
        }

        public static bool CheckIfHttpDatesMatch(string serverDate, string clientHeaderDate)
        {
            return NormalizeToFormatedHttpDate(clientHeaderDate) == NormalizeToFormatedHttpDate(serverDate);
        }
    }
}
