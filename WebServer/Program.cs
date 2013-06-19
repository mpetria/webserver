using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().Start(IPAddress.Any, 9010);
        }
    }
}
