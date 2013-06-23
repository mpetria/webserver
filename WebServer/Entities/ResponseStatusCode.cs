using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{

    public enum ResponseStatusCode
    {
        OK = 200,
        INTERNAL_SERVER_ERROR = 500
    }

    public class ResponseStatusDescription
    {
        public static Dictionary<ResponseStatusCode, string> Default = new Dictionary<ResponseStatusCode, string>()
                {
                    { ResponseStatusCode.OK, "OK" },
                    { ResponseStatusCode.INTERNAL_SERVER_ERROR, "Internal Server Error" }                                                 
                };
    }
}
