using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{
    public class Request
    {
        public string Method { get; set; }
        public string Host { get; set; }
        public string Uri { get; set; }
    }
}
