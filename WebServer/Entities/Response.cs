using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{
    public class Response
    {

        public Response()
        {
            Headers = new Dictionary<string, string>();
        }
        public string Status { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }


        public Dictionary<string, string> Headers { get; set; }
        
    }
}
