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

    public class Response
    {
        public Response()
        {
            Headers = new Dictionary<string, string>();
        }
        
        public string Body { get; set; }
        public string ContentType { get; set; }

        public ResponseStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }


        public Dictionary<string, string> Headers { get; set; }
        
    }
}
