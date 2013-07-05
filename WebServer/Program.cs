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
            var serverConfig = ServerConfigFactory.GetServerConfig();
            var server = new Server();
            server.Start(IPAddress.Any, serverConfig.Port);

            Console.WriteLine("Server started for {0}:{1}", serverConfig.Host, serverConfig.Port);
            Console.WriteLine("Root directory is {0}", serverConfig.RootDirectory);
            Console.WriteLine("Press ENTER to stop");

            Console.ReadLine();

            server.Stop();

        }
    }
}
