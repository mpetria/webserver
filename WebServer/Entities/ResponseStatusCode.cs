using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{

    public enum ResponseStatusCode
    {
        Ok = 200,
        InternalServerError = 500,
        NotModified =304
    }

    public class ResponseStatusDescription
    {
        public static Dictionary<ResponseStatusCode, string> Default = new Dictionary<ResponseStatusCode, string>()
                {
                    { ResponseStatusCode.Ok, "OK" },
                    { ResponseStatusCode.InternalServerError, "Internal Server Error" }                                                 
                };
    }
}
