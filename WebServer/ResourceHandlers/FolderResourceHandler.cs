using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Data;
using WebServer.Utils;

namespace WebServer.ResourceHandlers
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
            return new List<string>(){ HttpMethod.GET, HttpMethod.HEAD };
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

        override public bool GetVersioning(string resourceUri, out string lastUpdateDate, out string eTag)
        {
            eTag = null;

            var physicalPath = GetPhysicalPath(resourceUri);
            DateTime lastModifiedDate = File.GetLastWriteTimeUtc(physicalPath);
            lastUpdateDate = DateUtils.GetFormatedHttpDateFromUtcDate(lastModifiedDate);
            return true;
        }
    }
}
