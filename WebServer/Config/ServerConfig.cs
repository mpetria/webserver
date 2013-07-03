using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WebServer.ResourceHandlers;

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

        public string RootDirectory = @"C:\Work\SiteRoot";



        public string Host { get; set; }
        

        public IResourceHandler GetHandlerForPath(string host, string path)
        {
            if(!Path.HasExtension(path))
            {
                return new FolderResourceHandler(RootDirectory);
            }
            else
            {
                return new FileResourceHandler(RootDirectory);
            }
        }

        public int Port = 9010;
        public IPAddress IpAddress = IPAddress.Loopback;



        public int MaxUriLength = 512;

        public bool UseStreams = false;


        public bool IsSupportedHost(string host)
        {
            return true;
        }




        public static ServerConfig Instance = new ServerConfig();
    }

}
