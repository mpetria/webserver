﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebServer.Data
{


    public class Response
    {
        public Response()
        {
            Headers = new Dictionary<string, string>();
            SuppressBody = false;
        }
        
        public string Body { get; set; }
        public byte[] BodyBytes { get; set; }
        public Stream BodyStream { get; set; }
        public string ContentType { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public DateTime? LastModified { get; set; }


        public Dictionary<string, string> Headers { get; set; }

        public bool SuppressBody { get; set; }
        
    }
}
