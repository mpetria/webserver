using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public const string DefaultMimeType = "application/octet-stream";

        public int Port = 80;

        public int MaxUriLength = 512;

        public bool UseStreams = false;

        public readonly string RootDirectory = @"C:\SiteRoot";
       
        public readonly string Host = "*";


        public ServerConfig(NameValueCollection settings)
        {
            try
            {
                RootDirectory = settings["RootDirectory"];
                UseStreams = bool.Parse(settings["UseStreams"]);
                MaxUriLength = int.Parse(settings["MaxUriLength"]);
                Host = settings["Host"];
                Port = int.Parse(settings["Port"]);
            }
            catch (Exception)
            {
            }
        }

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


        public IResourceHandler GetHandlerForPath(string host, string path)
        {
            if(!Path.HasExtension(path))
            {
                return new FolderResourceHandler(RootDirectory);
            }
            else
            {
                return new FileResourceHandler(RootDirectory, this);
            }
        }

        public bool IsSupportedHost(string host)
        {
            if (Host == "*")
                return true;
            
            return String.Compare(Host, host, ignoreCase: true) == 0;
        }

        public bool IsSupportedTransferEncoding(string encoding)
        {
            var supportedEncodings = new [] {"chunked", "identity"};
            return supportedEncodings.Contains(encoding.ToLower());
        }

        public bool IsSupportedContentEncoding(string encoding)
        {
            var supportedEncodings = new[] { "identity" };
            return supportedEncodings.Contains(encoding.ToLower());
        }
    }

}
