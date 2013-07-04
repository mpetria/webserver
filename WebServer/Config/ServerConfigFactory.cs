using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WebServer.Config
{
    public class ServerConfigFactory
    {
        private static ServerConfig _instance; 

        static ServerConfigFactory()
        {
            // Get the AppSettings section.
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            _instance = new ServerConfig(appSettings);
        }
        
        public static ServerConfig GetServerConfig()
        {
            return _instance;
        }
    }
}
