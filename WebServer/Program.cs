using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebServer.Config;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().Start(ServerConfig.Instance.IpAddress, ServerConfig.Instance.Port);
        }
    }
}
