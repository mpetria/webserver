using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using WebServer.ResourceHandlers;
using WebServer.Utils;

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
                ReadMimeTypesFromResource();
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

        public bool IsSupportedMethod(string method)
        {
            var supportedMetohds = new[] {"GET", "HEAD", "PUT"};
            return supportedMetohds.Contains(method);
        }

        private void  ReadMimeTypesFromResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "WebServer.Config.mime.types";
            var dict = new Dictionary<string, string>();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                 using (StreamReader reader = new StreamReader(stream))
                 {
                     while (reader.Peek() >= 0)
                     {
                         var line = reader.ReadLine();
                         line = line.SubstringBefore('#');
                         line = line.Trim();
                         var tokens = line.Split(new char[] {'\t', ' '}, StringSplitOptions.RemoveEmptyEntries);
                         if(tokens.Length > 1)
                         {
                             for (int i = 1; i < tokens.Length; i++)
                             {
                                 string extension = tokens[i];
                                 string mime = tokens[0];
                                 dict[extension] = mime;
                             }
                             
                         }
                     }
                 }
            }
            ExtensionsToMimeTypes = dict;
        }
    
    }

}
