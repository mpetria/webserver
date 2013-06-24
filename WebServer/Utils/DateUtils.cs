using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Utils
{
    class DateUtils
    {
        public static string GetFormatedServerDate(DateTime dateTime)
        {
            //RFC 1123 Date
            return dateTime.ToUniversalTime().ToString("r");
        }

        public static bool CheckIfDatesMatch(DateTime dateTime, string headerDate)
        {
            return GetFormatedServerDate(dateTime) == headerDate;
        }
    }
}
