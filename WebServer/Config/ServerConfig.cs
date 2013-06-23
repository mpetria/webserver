using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Config
{
    public class ServerConfig
    {
        public Dictionary<string, string> ExtensionsToMimeTypes = new Dictionary<string, string>()
        {
            { "html", "text/html" },
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
    }
}
