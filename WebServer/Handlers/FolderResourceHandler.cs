using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Handlers
{
    public class FolderResourceHandler : FsResourceHandler
    {

        public FolderResourceHandler(string directory) : base(directory)
        {
           
        }

        override public bool CheckIfExists(string resourceUri)
        {
            var physicalPath = GetPhysicalPath(resourceUri);

            return Directory.Exists(physicalPath);
        }

        override public IList<string> GetAllowedMethods(string resourceUri)
        {
            return new List<string>(){ HTTPMethod.GET, HTTPMethod.HEAD };
        }

        private const string HtmlTemplate = @"<html><body>{0}</body></html>";
        override public byte[] GetResourceBytes(string resourceUri, string contentType)
        {
            StringBuilder sb = new StringBuilder();

            
            var physicalPath = GetPhysicalPath(resourceUri);
            foreach (var filePath in Directory.GetFiles(physicalPath))
            {
                var fileInfo = new FileInfo(filePath);
                sb.AppendFormat(@"<div><a href=""{0}"">{1}<a></div>", GetResourceUri(fileInfo.FullName), fileInfo.Name);

            }


            var response = String.Format(HtmlTemplate, sb);
            return new ASCIIEncoding().GetBytes(response);
        }

        override public IList<string> GetAvailableMediaTypes(string resourceUri, string method)
        {
            return new List<string>() { "text/html" };
        }
    }
}
