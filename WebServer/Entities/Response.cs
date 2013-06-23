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
        
        public string Body { get; set; }
        public byte[] BodyBytes { get; set; }
        public string ContentType { get; set; }

        public ResponseStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public DateTime? LastModified { get; set; }


        public Dictionary<string, string> Headers { get; set; }
        
    }
}
