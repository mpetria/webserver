using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebServer.Handlers;

namespace WebServer.Config
{
    public class ServerConfig
    {
        public Dictionary<string, string> ExtensionsToMimeTypes = new Dictionary<string, string>()
        {
            { "html", "text/html" },
            { "htm", "text/html" },
            { "jpg", "image/jpeg"},	
            { "jpeg", "image/jpeg"},	
			{ "png", "image/png"},
            { "txt", "text/plain"}
        };

        public string DefaultMimeType = "application/octet-stream";

        public string GetMimeTypeForExtension(string extension)
        {
            extension = extension ?? String.Empty;
            extension = extension.TrimStart(".".ToCharArray());
            extension = extension.ToLower();

            if (String.IsNullOrEmpty(extension) || !ExtensionsToMimeTypes.ContainsKey(extension))
            {
                return DefaultMimeType;
            }
            
            return ExtensionsToMimeTypes[extension];
        }

        public static string RootDirectory = @"C:\Work\SiteRoot";



        public string Host { get; set; }
        public Dictionary<string, Func<IRequestHandler>> HandlerMappings = new Dictionary<string, Func<IRequestHandler>>()
        {
            { "", () => new StaticAssetsHandler(RootDirectory) }
        };

        public IRequestHandler GetHandlerForPath(string path)
        {
            foreach (var handlerMapping in HandlerMappings)
            {
                if (path.StartsWith(handlerMapping.Key))
                    return handlerMapping.Value();
            }
            return null;
        }

        public int Port = 9010;
        public IPAddress IpAddress = IPAddress.Loopback;




        public static ServerConfig Instance = new ServerConfig();
    }

}
